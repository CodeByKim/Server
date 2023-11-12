using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using Core.Server;
using Core.Util;

namespace Core.Connection
{
    public class ClientConnection<TConnection> : BaseConnection
        where TConnection : ClientConnection<TConnection>, new()
    {
        private BaseServer<TConnection> _server;

        public ClientConnection() : base()
        {
        }

        public void Initialize(Socket socket, BaseServer<TConnection> server)
        {
            Initialize(socket);

            _server = server;
        }

        protected override void OnDisconnected(BaseConnection conn, DisconnectReason reason)
        {
            _server.Disconnect(conn as TConnection, reason);
        }
    }
}
