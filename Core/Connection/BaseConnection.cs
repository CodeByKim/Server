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
    public abstract class BaseConnection
    {
        protected Socket _socket;

        private byte[] _receiveBuffer;

        public BaseConnection()
        {
        }

        public void Send(string message)
        {
            var sendBytes = Encoding.UTF8.GetBytes("Hello World");

            try
            {
                _socket.Send(sendBytes);
            }
            catch (Exception e)
            {
                OnDisconnected(this, DisconnectReason.RemoteClosing);
            }
        }

        internal void Initialize(Socket socket)
        {
            _socket = socket;

            _receiveBuffer = new byte[ServerConfig.Instance.ReceiveBufferSize];
        }

        internal void Release()
        {
            Logger.Info($"Release Connection: {GetHashCode()}");
        }

        internal async void ReceiveAsync()
        {
            var tempBuffer = new ArraySegment<byte>(_receiveBuffer);

            try
            {
                var byteTransfer = await _socket.ReceiveAsync(tempBuffer);
                var message = Encoding.UTF8.GetString(tempBuffer.Array, 0, byteTransfer);

                Logger.Info($"From: {GetHashCode()}, message: {message}");

                Array.Clear(_receiveBuffer);
                ReceiveAsync();

            }
            catch (Exception e)
            {
                OnDisconnected(this, DisconnectReason.RemoteClosing);
            }
        }

        protected abstract void OnDisconnected(BaseConnection conn, DisconnectReason reason);
    }
}