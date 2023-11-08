using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Core.Connection
{
    public class Acceptor
    {
        private Socket _socket;
        private IPEndPoint _endPoint;

        public void Initialize()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var port = 8888;
            _endPoint = new IPEndPoint(IPAddress.Any, port);
        }

        public async void Run()
        {
            _socket.Bind(_endPoint);

            var backlog = 100;
            _socket.Listen(backlog);

            while (true)
            {
                Console.WriteLine("Listen...: " + Thread.CurrentThread.ManagedThreadId);
                var clientSocket = await _socket.AcceptAsync();

                OnNewClient(clientSocket);
            }
        }

        private void OnNewClient(Socket socket)
        {
            Console.WriteLine("OnNewClient...: " + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Hello NewSocket: " + socket.GetHashCode());
        }
    }
}
