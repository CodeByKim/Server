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

    public override void ExecutePacketHandler(GameConnection conn, short packetId, ArraySegment<byte> payload)
    {
        switch ((PacketId)packetId)
        {
            case PacketId.PktEcho:
                {
                    var packet = new PktEcho();
                    packet.MergeFrom(payload);

                    OnPktEchoHandler(conn, packet);
                }
                break;
        }
    }

    private void OnPktEchoHandler(GameConnection conn, PktEcho pkt)
    {
        Logger.Info($"From: {conn.ID}, message: {pkt.Message}");

        PktEchoResult pktResult = new PktEchoResult();
        pktResult.Message = "good";

        conn.Send((short)PacketId.PktEchoResult, pktResult);
    }
}