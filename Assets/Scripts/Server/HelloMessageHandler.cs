using App.Messages;
using Mirror;
using System.Collections.Generic;

namespace App.Server
{
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
