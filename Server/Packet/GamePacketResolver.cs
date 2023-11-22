using System;
using System.Collections.Generic;

using Core.Packet;
using Core.Util;
using Google.Protobuf;
using Protocol;

public class GamePacketResolver : AbstractPacketResolver<GameConnection>
{
    public GamePacketResolver()
    {
    }

    protected override void OnRegisterPacketHandler(Dictionary<short, AbstractPacketHandler<GameConnection>> handlers)
    {
        handlers.Add((short)PacketId.PktEcho, new PktEchoHandler());
    }

    public override IMessage? OnResolvePacket(GameConnection conn, short packetId)
    {
        if (!ContainHandler(packetId))
            return null;

        switch ((PacketId)packetId)
        {
            case PacketId.PktEcho:
                return new PktEcho();
        }

        return null;
    }
}