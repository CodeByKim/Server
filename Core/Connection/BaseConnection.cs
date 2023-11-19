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

        protected Socket _socket;

        private RingBuffer _receiveBuffer;
        private bool _isSending;
        private object _sendLock;

        private List<ArraySegment<byte>> _reservedSendList;

        public BaseConnection()
        {
            ID = Guid.NewGuid().ToString();

            _sendLock = new object();
            _reservedSendList = new List<ArraySegment<byte>>();

            _isSending = false;

            //_receiveBuffer = new RingBuffer(ServerConfig.Instance.ReceiveBufferSize);
            _receiveBuffer = new RingBuffer(15);
        }

        public void Send(short packetId, IMessage packet)
        {
            var header = new PacketHeader(packet, packetId);
            var buffer = PacketUtil.CreateBuffer(header, packet);

            lock (_sendLock)
            {
                _reservedSendList.Add(buffer);

                if (_isSending)
                    return;

                TrySend();
            }
        }

        internal void Initialize(Socket socket)
        {
            _socket = socket;
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
                OnDispatchPacket(header, payload);

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
            List<ArraySegment<byte>> sendList;
            sendList = _reservedSendList;

            _reservedSendList = new List<ArraySegment<byte>>();

            SendAsync(sendList);
        }

        private async void SendAsync(List<ArraySegment<byte>> sendList)
        {
            try
            {
                _isSending = true;
                await _socket.SendAsync(sendList);
            }
            catch (Exception e)
            {
                ForceDisconnect(DisconnectReason.RemoteClosing);
            }

            lock (_sendLock)
            {
                //완료 처리
                _isSending = false;

                if (_reservedSendList.Count > 0)
                    TrySend();
            }
        }

        private bool TryGetHeader(out PacketHeader header)
        {
            header = new PacketHeader();

            if (_receiveBuffer.UseSize < PacketHeader.HeaderSize)
                return false;

            header.CopyTo(_receiveBuffer);
            return true;
        }

        protected abstract void OnDispatchPacket(PacketHeader header, ArraySegment<Byte> data);

        protected abstract void OnDisconnected(BaseConnection conn, DisconnectReason reason);
    }
}