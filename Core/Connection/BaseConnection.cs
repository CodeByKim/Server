using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Google.Protobuf;

using Core.Buffer;
using Core.Server;
using Core.Util;
using Core.Packet;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Connection
{
    public abstract class BaseConnection
    {
        public string ID { get; }
        public Func<IMessage, short> OnGetPacketIdHandler { get; set; }

        protected Socket _socket;

        private RingBuffer _receiveBuffer;
        private List<ArraySegment<byte>> _reserveSendList;
        private bool _isSending;

        private object _sendLock;

        public BaseConnection()
        {
            ID = Guid.NewGuid().ToString();

            _sendLock = new object();
            _reserveSendList = new List<ArraySegment<byte>>();
            _isSending = false;
        }

        public void Send(IMessage packet)
        {
            var packetId = OnGetPacketIdHandler(packet);

            var header = new PacketHeader(packet, packetId);
            var buffer = PacketUtil.CreateBuffer(header, packet);

            lock (_sendLock)
            {
                _reserveSendList.Add(buffer);

                if (_isSending)
                    return;
            }

            TrySend();
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
                PacketHeader header;
                if (!TryGetHeader(out header))
                    return;

                if (_receiveBuffer.UseSize < header.Payload)
                    return;

                var packetSize = PacketHeader.HeaderSize + header.Payload;
                var packetBuffer = _receiveBuffer.Peek(packetSize);
                if (packetBuffer.Array is null)
                {
                    ForceDisconnect(DisconnectReason.InvalidConnection);
                    return;
                }

                var payload = new ArraySegment<byte>(packetBuffer.Array,
                                                     packetBuffer.Offset + PacketHeader.HeaderSize,
                                                     header.Payload);
                OnParsePacket(header, payload);

                _receiveBuffer.FinishRead(packetSize);
            }
        }

        internal void ForceDisconnect(DisconnectReason reason)
        {
            if (_socket is null)
                Logger.Warnning("socket is null...");
            else
                _socket.Close();

            OnDisconnected(this, reason);
        }

        private void TrySend()
        {
            lock (_sendLock)
            {
                var sendList = new List<ArraySegment<byte>>();

                foreach (var item in _reserveSendList)
                    sendList.Add(item);

                _reserveSendList.Clear();

                SendAsync(sendList);
            }
        }

        private async void SendAsync(List<ArraySegment<byte>> sendList)
        {
            await _socket.SendAsync(sendList);

            //완료 처리
        }

        private bool TryGetHeader(out PacketHeader header)
        {
            header = new PacketHeader();

            if (_receiveBuffer.UseSize < PacketHeader.HeaderSize)
                return false;

            header.CopyTo(_receiveBuffer);
            return true;
        }

        protected abstract void OnParsePacket(PacketHeader header, ArraySegment<Byte> data);

        protected abstract void OnDisconnected(BaseConnection conn, DisconnectReason reason);
    }
}