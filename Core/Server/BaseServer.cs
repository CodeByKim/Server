using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using Core.Connection;
using Microsoft.Extensions.ObjectPool;
using Core.Util;

namespace Core.Server
{
    public abstract class BaseServer<TConnection>
        where TConnection : BaseConnection<TConnection>, new()
    {
        private Acceptor _acceptor;
        private DefaultObjectPool<TConnection> _connectionPool;

        public BaseServer()
        {
            _acceptor = new Acceptor();
            _connectionPool = new DefaultObjectPool<TConnection>(new ConnectionPooledObjectPolicy<TConnection>(), 100);
        }

        public virtual void Initialize(string configPath)
        {
            ServerConfig.Load(configPath);

            _acceptor.Initialize(AcceptNewClient);
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

        protected abstract void OnNewConnection(TConnection conn);

        protected abstract void OnDisconnected(TConnection conn, DisconnectReason reason);
    }
}
