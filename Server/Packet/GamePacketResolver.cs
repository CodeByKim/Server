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

    public override void OnResolvePacket(GameConnection conn, short packetId, ArraySegment<byte> payload)
    {
        if (!ContainHandler(packetId))
        {
            Logger.Error($"Not Found Packet Handler, PacketId: {packetId}");
            return;
        }

        IMessage packet = null;

        switch ((PacketId)packetId)
        {
            case PacketId.PktEcho:
                packet = new PktEcho();
                break;
        }

        packet.MergeFrom(payload);
        OnExecute(conn, packetId, packet);
    }
}