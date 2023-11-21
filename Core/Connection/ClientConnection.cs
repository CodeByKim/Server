using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using Core.Server;
using Core.Util;
using Core.Packet;

namespace Core.Connection
{
    public abstract class ClientConnection<TConnection> : BaseConnection
        where TConnection : ClientConnection<TConnection>, new()
    {
        private BaseServer<TConnection> _server;
        private AbstractPacketResolver<TConnection> _packetResolver;

        public ClientConnection() : base()
        {
        }

        public void Initialize(Socket socket, BaseServer<TConnection> server)
        {
            Initialize(socket);

            _server = server;

            _packetResolver = _server.OnGetPacketResolver();
        }

        protected override void OnDispatchPacket(PacketHeader header, ArraySegment<byte> payload)
        {
            _packetResolver.OnResolvePacket(this as TConnection, header.PacketId, payload);
        }

        protected override void OnDisconnected(BaseConnection conn, DisconnectReason reason)
        {
            _server.Disconnect(conn as TConnection, reason);
        }
    }
}
