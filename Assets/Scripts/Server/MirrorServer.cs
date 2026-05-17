using App.Messages;
using Mirror;
using UnityEngine;

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
}
