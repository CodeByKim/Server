using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Core.Packet;
using Core.Server;
using Core.Util;

namespace Core.Connection
{
    public abstract class Connector<TConnection> : BaseConnection
        where TConnection : BaseConnection
    {
        private AbstractPacketResolver<TConnection> _packetResolver;

        public Connector() : base()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _packetResolver = OnRegisterPacketResolver();
        }

        public async Task ConnectAsync(string ip, int portNumber)
        {
            await _socket.ConnectAsync(ip, portNumber);

            ReceiveAsync();
        }

        protected override void OnDispatchPacket(PacketHeader header, ArraySegment<byte> payload)
        {
            _packetResolver.ExecutePacketHandler(this as TConnection, header.PacketId, payload);
        }

        protected abstract AbstractPacketResolver<TConnection> OnRegisterPacketResolver();
    }
}
