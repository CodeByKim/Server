using Core.Connection;
using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace Core.Packet
{
    public abstract class AbstractPacketResolver<TConnection> where TConnection : BaseConnection
    {
        private Dictionary<short, AbstractPacketHandler<TConnection>> _packetHandlers;

        public AbstractPacketResolver()
        {
            _packetHandlers = new Dictionary<short, AbstractPacketHandler<TConnection>>();
            OnRegisterPacketHandler(_packetHandlers);
        }

        protected bool ContainHandler(short packetId)
        {
            return _packetHandlers.ContainsKey(packetId);
        }

        internal void Execute(TConnection conn, short packetId, IMessage packet)
        {
            var handler = _packetHandlers[packetId];
            handler.OnHandle(conn, packet);
        }

        public abstract IMessage? OnResolvePacket(TConnection conn, short packetId);

        protected abstract void OnRegisterPacketHandler(Dictionary<short, AbstractPacketHandler<TConnection>> handlers);
    }
}
