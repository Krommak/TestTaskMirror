using App.Client;
using App.Server;
using System.Threading.Tasks;
using Zenject;

namespace App
{
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
