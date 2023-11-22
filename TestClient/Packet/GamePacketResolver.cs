using System;
using System.Collections.Generic;

using Core.Packet;
using Core.Util;
using Google.Protobuf;
using Protocol;

public class GamePacketResolver : AbstractPacketResolver<DummyConnector>
{
    public GamePacketResolver()
    {
    }

    protected override void OnRegisterPacketHandler(Dictionary<short, AbstractPacketHandler<DummyConnector>> handlers)
    {
        handlers.Add((short)PacketId.PktEchoResult, new PktEchoResultHandler());
    }

    public override IMessage? OnResolvePacket(DummyConnector conn, short packetId)
    {
        if (!ContainHandler(packetId))
            return null;

        switch ((PacketId)packetId)
        {
            case PacketId.PktEchoResult:
                return new PktEchoResult();
        }

        return null;
    }
}