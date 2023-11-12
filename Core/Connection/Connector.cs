using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

using Core.Server;

namespace Core.Connection
{
    public class Connector : BaseConnection
    {
        public Action<Connector, DisconnectReason> OnDisconnectedHandler { get; set; }

        public Connector()
        {
        }

        public void Initialize()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConnectAsync(string ip, int portNumber)
        {
            await _socket.ConnectAsync(ip, portNumber);
        }

        protected override void OnDisconnected(BaseConnection conn, DisconnectReason reason)
        {
            if (OnDisconnectedHandler is null)
                return;

            OnDisconnectedHandler(this, reason);
        }
    }
}
