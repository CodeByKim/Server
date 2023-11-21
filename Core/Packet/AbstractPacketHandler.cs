using System;
using System.Collections.Generic;

using Core.Connection;
using Google.Protobuf;

namespace Core.Packet
{
    public abstract class AbstractPacketHandler<TConnection> where TConnection : BaseConnection
    {
        public abstract void OnHandle(TConnection conn, IMessage packet);
    }
}
