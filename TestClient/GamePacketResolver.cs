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

    public override void ExecutePacketHandler(DummyConnector conn, short packetId, ArraySegment<byte> payload)
    {
        switch ((PacketId)packetId)
        {
            case PacketId.PktEchoResult:
                {
                    var packet = new PktEchoResult();
                    packet.MergeFrom(payload);

                    OnPktEchoResultHandler(conn, packet);
                }
                break;
        }
    }

    private void OnPktEchoResultHandler(DummyConnector conn, PktEchoResult pkt)
    {
        Logger.Info($"From: {conn.ID}, message: {pkt.Message}");
    }
}