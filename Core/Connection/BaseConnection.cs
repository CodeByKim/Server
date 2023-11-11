using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Core.Server;
using Core.Util;

namespace Core.Connection
{
    public class BaseConnection<TConnection>
        where TConnection : BaseConnection<TConnection>, new()
    {
        private Socket _socket;
        private BaseServer<TConnection> _server;

        public BaseConnection()
        {
        }

        internal void Initialize(Socket socket, BaseServer<TConnection> server)
        {
            _socket = socket;
            _server = server;
        }

        internal void Release()
        {
            Logger.Info($"Release Connection: {GetHashCode()}");
        }

        internal async void ReceiveAsync()
        {
            var tempBuffer = new ArraySegment<byte>(new byte[1024]);

            try
            {
                var byteTransfer = await _socket.ReceiveAsync(tempBuffer);
                var message = Encoding.UTF8.GetString(tempBuffer.Array, 0, byteTransfer);

                Logger.Info($"From: {GetHashCode()}, message: {message}");

                ReceiveAsync();

            }
            catch (Exception e)
            {
                _server.Disconnect(this as TConnection, DisconnectReason.RemoteClosing);
            }
        }
    }
}