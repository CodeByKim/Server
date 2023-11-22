using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using Core.Server;
using Core.Util;
using Core.Packet;
using Google.Protobuf;


namespace Core.Connection
{
    public abstract class ClientConnection<TConnection> : BaseConnection
        where TConnection : ClientConnection<TConnection>, new()
    {
        private BaseServer<TConnection> _server;
        private AbstractPacketResolver<TConnection> _packetResolver;

        private ConcurrentQueue<Tuple<short, IMessage>> _packetQueue;

        public ClientConnection() : base()
        {
        }

        public void Initialize(Socket socket, BaseServer<TConnection> server)
        {
            Initialize(socket);

            _server = server;

            _packetResolver = _server.OnGetPacketResolver();

            _packetQueue = new ConcurrentQueue<Tuple<short, IMessage>>();
        }

        internal void ConsumePacket()
        {
            while (_packetQueue.Count > 0)
            {
                Tuple<short, IMessage> packetBundle;
                if (!_packetQueue.TryDequeue(out packetBundle))
                    return;

                _packetResolver.Execute(this as TConnection, packetBundle.Item1, packetBundle.Item2);
            }
        }

        protected override void OnDispatchPacket(PacketHeader header, ArraySegment<byte> payload)
        {
            var conn = this as TConnection;
            var packetId = header.PacketId;
            var packet = _packetResolver.OnResolvePacket(conn, packetId);
            if (packet is null)
            {
                Logger.Error($"Not Found Packet Handler, PacketId: {packetId}");
                return;
            }

            packet.MergeFrom(payload);

            var packetBundle = new Tuple<short, IMessage>(packetId, packet);
            _packetQueue.Enqueue(packetBundle);
        }

        protected override void OnDisconnected(BaseConnection conn, DisconnectReason reason)
        {
            _server.Disconnect(conn as TConnection, reason);
        }
    }
}
