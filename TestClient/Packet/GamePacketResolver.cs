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

    public override void OnResolvePacket(DummyConnector conn, short packetId, ArraySegment<byte> payload)
    {
        if (!ContainHandler(packetId))
        {
            Logger.Error($"Not Found Packet Handler, PacketId: {packetId}");
            return;
        }

        IMessage packet = null;

        switch ((PacketId)packetId)
        {
            case PacketId.PktEchoResult:
                packet = new PktEchoResult();
                break;
        }

        packet.MergeFrom(payload);
        OnExecute(conn, packetId, packet);
    }
}