using System.Net;
using System.Net.Sockets;

using Core.Server;
using Core.Util;

namespace Core.Connection
{
    public class Acceptor
    {
        public Action<Socket> OnNewClientHandler { get; set; }

        private Socket _socket;
        private IPEndPoint _endPoint;

        public Acceptor()
        {
        }

        public void Initialize()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _endPoint = new IPEndPoint(IPAddress.Any, ServerConfig.Instance.PortNumber);
        }

        public async void Run()
        {
            _socket.Bind(_endPoint);

            _socket.Listen(ServerConfig.Instance.Backlog);

            while (true)
            {
                var clientSocket = await _socket.AcceptAsync();

                OnNewClientHandler(clientSocket);
            }
        }
    }
}
