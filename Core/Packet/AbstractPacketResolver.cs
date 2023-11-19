using Core.Connection;
using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace Core.Packet
{
    public abstract class AbstractPacketResolver<TConnection> where TConnection : BaseConnection
    {
        public AbstractPacketResolver()
        {
        }

        public abstract void ExecutePacketHandler(TConnection conn, short packetId, ArraySegment<byte> payload);
    }
}
