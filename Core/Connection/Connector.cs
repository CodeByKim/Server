using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Core.Packet;
using Core.Server;
using Core.Util;

namespace Core.Connection
{
    public class Connector : BaseConnection
    {
        public Action<DisconnectReason> OnDisconnectedHandler { get; set; }

        public Connector() : base()
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
            {
                Logger.Warnning("OnDisconnectedHandler is null");
                return;
            }

            OnDisconnectedHandler(reason);
        }

        protected override void OnParsePacket(PacketHeader header, ArraySegment<byte> data)
        {
        }
    }
}
