using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Google.Protobuf;

using Core.Buffer;
using Core.Server;
using Core.Util;
using Core.Packet;

namespace Core.Connection
{
    public abstract class BaseConnection
    {
        public string ID { get; }
        public Func<IMessage, short> OnGetPacketIdHandler { get; set; }

        protected Socket _socket;

        private RingBuffer _receiveBuffer;
        private List<ArraySegment<byte>> _reserveSendList;

        private object _sendLock;

        public BaseConnection()
        {
            ID = Guid.NewGuid().ToString();

            _sendLock = new object();
            _reserveSendList = new List<ArraySegment<byte>>();
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
                ForceDisconnect(DisconnectReason.RemoteClosing);
            }
        }

        public void Send(IMessage packet)
        {
            var packetId = OnGetPacketIdHandler(packet);

            var header = new PacketHeader(packet, packetId);
            var buffer = PacketUtil.CreateBuffer(header, packet);

            lock (_sendLock)
            {
                _reserveSendList.Add(buffer);
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
            var writableSegments = _receiveBuffer.GetWritable();

            try
            {
                var byteTransfer = await _socket.ReceiveAsync(writableSegments);

                ProcessReceive(byteTransfer);

                ReceiveAsync();

            }
            catch (Exception e)
            {
                ForceDisconnect(DisconnectReason.RemoteClosing);
            }
        }

        internal void ProcessReceive(int byteTransfer)
        {
            _receiveBuffer.FinishWrite(byteTransfer);

            if (byteTransfer == 0 || _receiveBuffer.UseSize == 0)
                return;

            while (_receiveBuffer.UseSize > 0)
            {
                var payload = 11; // 임시로 hello world의 사이즈를 박아서 테스트
                if (_receiveBuffer.UseSize < payload)
                    return;

                var data = _receiveBuffer.Peek(payload);
                if (data.Array is null)
                {
                    ForceDisconnect(DisconnectReason.InvalidConnection);
                    return;
                }

                ParsePacket(data, payload);

                // 나중엔 실제로 처리된 바이트를 읽음 처리해야 한다.
                _receiveBuffer.FinishRead(payload);
            }
        }

        internal void ParsePacket(ArraySegment<Byte> data, int payload)
        {
            var message = Encoding.UTF8.GetString(data.Array, data.Offset, payload);

            Logger.Info($"From: {ID}, message: {message}");
        }

        internal void ForceDisconnect(DisconnectReason reason)
        {
            if (_socket is null)
                Logger.Warnning("socket is null...");
            else
                _socket.Close();

            OnDisconnected(this, reason);
        }

        protected abstract void OnDisconnected(BaseConnection conn, DisconnectReason reason);
    }
}