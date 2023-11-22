using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using Microsoft.Extensions.ObjectPool;

using Core.Connection;
using Core.Util;
using Core.Packet;
using Core.Logic;

namespace Core.Server
{
    public abstract class BaseServer<TConnection>
        where TConnection : ClientConnection<TConnection>, new()
    {
        private Acceptor _acceptor;
        private DefaultObjectPool<TConnection> _connectionPool;
        private List<Room<TConnection>> _rooms;

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

            _rooms = new List<Room<TConnection>>();
        }

        public void Run()
        {
            _acceptor.Run();

            RunRoomLogic();
        }

        internal void AcceptNewClient(Socket socket)
        {
            var conn = _connectionPool.Get();
            conn.Initialize(socket, this);

            OnNewConnection(conn);

            // 코드 이상하다.
            foreach (var room in _rooms)
                room.Add(conn);

            conn.ReceiveAsync();
        }

        internal void Disconnect(TConnection conn, DisconnectReason reason)
        {
            OnDisconnected(conn, reason);

            _connectionPool.Return(conn);
        }

        private void RunRoomLogic()
        {
            var roomCount = ServerConfig.Instance.RoomCount;
            for (var i = 0; i < roomCount; i++)
            {
                var room = new Room<TConnection>();
                _rooms.Add(room);

                ThreadPool.QueueUserWorkItem(room.OnRun);
            }
        }

        public abstract AbstractPacketResolver<TConnection> OnGetPacketResolver();

        protected abstract void OnNewConnection(TConnection conn);

        protected abstract void OnDisconnected(TConnection conn, DisconnectReason reason);
    }
}
