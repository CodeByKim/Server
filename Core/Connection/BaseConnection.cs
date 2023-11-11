using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Core.Server;
using System.Threading.Channels;
using System.Text;
using Core.Util;

namespace Core.Connection
{
    public class BaseConnection
    {
        private Socket _socket;
        private BaseServer _server;

        public BaseConnection(Socket socket, BaseServer server)
        {
            _socket = socket;
            _server = server;
        }

        internal async void ReceiveAsync()
        {
            var tempBuffer = new ArraySegment<byte>(new byte[1024]);

            try
            {
                var bytesRead = await _socket.ReceiveAsync(tempBuffer);
                var message = Encoding.UTF8.GetString(tempBuffer.Array, 0, bytesRead);

                Logger.Info($"From: {GetHashCode()}, message: {message}");

                ReceiveAsync();

            }
            catch (Exception e)
            {
                _server.Disconnect(this, DisconnectReason.RemoteClosing);
            }
        }
    }
}