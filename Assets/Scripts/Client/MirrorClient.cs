using App.Messages;
using Mirror;
using UnityEngine;

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
