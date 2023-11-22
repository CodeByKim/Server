using System;
using System.Collections.Generic;
using System.Net.Sockets;

using Core.Packet;
using Core.Server;
using Core.Util;
using Google.Protobuf;

namespace Core.Connection
{
    public abstract class Connector<TConnection> : BaseConnection
        where TConnection : BaseConnection
    {
        private AbstractPacketResolver<TConnection> _packetResolver;

        public Connector() : base()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _packetResolver = OnGetPacketResolver();
        }

        public async Task ConnectAsync(string ip, int portNumber)
        {
            await _socket.ConnectAsync(ip, portNumber);

            ReceiveAsync();
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
            _packetResolver.Execute(conn, packetId, packet);
        }

        protected abstract AbstractPacketResolver<TConnection> OnGetPacketResolver();
    }
}
