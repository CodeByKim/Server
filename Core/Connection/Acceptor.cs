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
        private BaseServer _server;

        public Acceptor(BaseServer server)
        {
            _server = server;
        }

        public void Initialize()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _endPoint = new IPEndPoint(IPAddress.Any, BaseServer.Config.PortNumber);
        }

        public async void Run()
        {
            _socket.Bind(_endPoint);

            _socket.Listen(BaseServer.Config.Backlog);

            while (true)
            {
                var clientSocket = await _socket.AcceptAsync();

                OnNewClient(clientSocket);
            }
        }

        private void OnNewClient(Socket socket)
        {
            _server.AcceptNewClient(socket);
        }
    }
}
