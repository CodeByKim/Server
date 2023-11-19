using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using Microsoft.Extensions.ObjectPool;

using Core.Connection;
using Core.Util;
using Core.Packet;

namespace Core.Server
{
    public abstract class BaseServer<TConnection>
        where TConnection : ClientConnection<TConnection>, new()
    {
        private Acceptor _acceptor;
        private DefaultObjectPool<TConnection> _connectionPool;
        private AbstractPacketResolver<TConnection> _packetResolver;

        public BaseServer(string configPath)
        {
            ServerConfig.Load(configPath);
        }

        public virtual void Initialize()
        {
            _acceptor = new Acceptor();
            _connectionPool = new DefaultObjectPool<TConnection>(new ConnectionPooledObjectPolicy<TConnection>(),
                                                                 ServerConfig.Instance.ConnectionPoolCount);
            _acceptor.Initialize();
            _acceptor.OnNewClientHandler = AcceptNewClient;

            _packetResolver = OnRegisterPacketResolver();
        }

        public void Run()
        {
            _acceptor.Run();
        }

        internal void AcceptNewClient(Socket socket)
        {
            var conn = _connectionPool.Get();
            conn.Initialize(socket, this);

            OnNewConnection(conn);

            conn.ReceiveAsync();
        }

        internal void Disconnect(TConnection conn, DisconnectReason reason)
        {
            OnDisconnected(conn, reason);

            _connectionPool.Return(conn);
        }

        internal void ParsePacket(TConnection conn, in PacketHeader header, ArraySegment<byte> payload)
        {
            _packetResolver.ExecutePacketHandler(conn, header.PacketId, payload);
        }

        protected abstract void OnNewConnection(TConnection conn);

        protected abstract void OnDisconnected(TConnection conn, DisconnectReason reason);

        protected abstract AbstractPacketResolver<TConnection> OnRegisterPacketResolver();
    }
}
