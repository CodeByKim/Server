using System.Net;
using System.Net.Sockets;

using Core.Server;
using Core.Util;

namespace Core.Connection
{
    public class Acceptor
    {
        private Socket _socket;
        private IPEndPoint _endPoint;
        private Action<Socket> _onNewClient;

        public Acceptor()
        {
        }

        public void Initialize(Action<Socket> onNewClient)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _endPoint = new IPEndPoint(IPAddress.Any, ServerConfig.Instance.PortNumber);

            _onNewClient = onNewClient;
        }

        public async void Run()
        {
            _socket.Bind(_endPoint);

            _socket.Listen(ServerConfig.Instance.Backlog);

            while (true)
            {
                var clientSocket = await _socket.AcceptAsync();

                _onNewClient(clientSocket);
            }
        }
    }
}
