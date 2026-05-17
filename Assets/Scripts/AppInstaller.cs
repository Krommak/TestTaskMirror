using App.Client;
using App.Messages;
using App.Server;
using Mirror;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace App
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<HelloMessageHandler>().AsSingle();
            Container.Bind<MirrorServer>().AsSingle();
            Container.Bind<MirrorClient>().AsSingle();

            Container.BindInterfacesAndSelfTo<AppStart>().AsSingle().NonLazy();
        }
    }

    public class AppStart : IInitializable
    {
        private MirrorServer _server;
        private MirrorClient _client;

        private string _address = "localhost";

        public AppStart(MirrorServer server, MirrorClient client)
        {
            _server = server;
            _client = client;
        }

        public async void Initialize()
        {
            _server.StartServer(_address);

            await Task.Delay(500);

            _client.Connect(_address);
        }
    }
}

namespace App.Server
{
    public class MirrorServer
    {
        private HelloMessageHandler _handler;

        public MirrorServer(HelloMessageHandler handler)
        {
            _handler = handler;
        }

        public void StartServer(string adress)
        {
            Debug.Log("[Server] Host started...");
            NetworkManager.singleton.StartHost();
            NetworkServer.OnConnectedEvent += OnClientConnected;
            NetworkServer.OnDisconnectedEvent += OnClientDisconnected;
        }

        public void Dispose()
        {
            NetworkServer.OnConnectedEvent -= OnClientConnected;
            NetworkServer.OnDisconnectedEvent -= OnClientDisconnected;
        }

        private void OnClientConnected(NetworkConnectionToClient connection)
        {
            Debug.Log($"[Server] connection {connection.connectionId}");
            _handler.RegisterClient(connection.connectionId, connection);

            _handler.BroadcastToSubscribers(new HelloMessage() { Text = "Hello Client!" });
        }

        private void OnClientDisconnected(NetworkConnectionToClient connection)
        {
            Debug.Log($"[Server] disconnection {connection.connectionId}");
            _handler.UnregisterClient(connection.connectionId);
        }
    }

    public class HelloMessageHandler
    {
        private readonly Dictionary<int, NetworkConnection> _subscribers = new();

        public void RegisterClient(int connId, NetworkConnection conn)
        {
            _subscribers[connId] = conn;
        }

        public void UnregisterClient(int connId)
        {
            _subscribers.Remove(connId);
        }

        public void BroadcastToSubscribers(HelloMessage message)
        {
            foreach (var conn in _subscribers)
            {
                NetworkServer.SendToReady(message, conn.Key);
            }
        }
    }
}

namespace App.Client
{
    public class MirrorClient
    {
        public void Connect(string adress)
        {
            Debug.Log("[Client] Client started...");
            var transport = Transport.active;
            transport.OnClientDataReceived = (segment, channel) =>
            {
            };
            transport.OnClientError = (TransportError error, string exception) => Debug.LogError($"Ошибка: {exception}");
            Transport.active.OnClientConnected += OnConnected;
            Transport.active.OnClientDisconnected += OnDisconnected;
            NetworkClient.RegisterHandler<HelloMessage>(OnHelloMessage);
            transport.ClientConnect(adress);
        }

        private void OnConnected()
        {
            Debug.Log("[Client] connected");
        }

        private void OnDisconnected()
        {
            Transport.active.OnClientConnected -= OnConnected;
            Transport.active.OnClientDisconnected -= OnDisconnected;

            NetworkClient.UnregisterHandler<HelloMessage>();
        }

        private void OnHelloMessage(HelloMessage message)
        {
            Debug.Log($"{message.Text}");
        }
    }
}

namespace App.Messages
{
    public struct HelloMessage : NetworkMessage
    {
        public string Text;
    }
}