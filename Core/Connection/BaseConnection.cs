using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Core.Buffer;
using Core.Server;
using Core.Util;

namespace Core.Connection
{
    public abstract class BaseConnection
    {
        public string ID { get; }

        protected Socket _socket;

        private RingBuffer _receiveBuffer;

        public BaseConnection()
        {
            ID = Guid.NewGuid().ToString();
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

            _receiveBuffer = new RingBuffer(ServerConfig.Instance.ReceiveBufferSize);
        }

        internal void Release()
        {
            Logger.Info($"Release Connection: {ID}");
        }

        internal async void ReceiveAsync()
        {
            var writableSegment = _receiveBuffer.GetWritable();

            try
            {
                var byteTransfer = await _socket.ReceiveAsync(writableSegment);

                ProcessReceive(byteTransfer);

                ReceiveAsync();

            }
            catch (Exception e)
            {
                OnDisconnected(this, DisconnectReason.RemoteClosing);
            }
        }

        internal void ProcessReceive(int byteTransfer)
        {
            _receiveBuffer.FinishWrite(byteTransfer);

            var packetSize = 11; // 임시로 hello world의 사이즈를 박아서 테스트
            if (_receiveBuffer.UseSize < packetSize)
                return;

            var data = _receiveBuffer.Peek(packetSize);
            if (data.Array is null)
            {
                OnDisconnected(this, DisconnectReason.InvalidConnection);
                return;
            }

            var message = Encoding.UTF8.GetString(data.Array, data.Offset, packetSize);
            Logger.Info($"From: {ID}, message: {message}");

            // 나중엔 실제로 처리된 바이트를 읽음 처리해야 한다.
            _receiveBuffer.FinishRead(packetSize);
        }

        protected abstract void OnDisconnected(BaseConnection conn, DisconnectReason reason);
    }
}