using System;
using System.Threading.Tasks;

using Core.Packet;
using Core.Util;
using Google.Protobuf;
using Protocol;

public class PktEchoHandler : AbstractPacketHandler<GameConnection>
{
    public override void OnHandle(GameConnection conn, IMessage packet)
    {
        var pkt = packet as PktEcho;
        Logger.Info($"From: {conn.ID}, message: {pkt.Message}");

        PktEchoResult pktResult = new PktEchoResult();
        pktResult.Message = "good";

        conn.Send((short)PacketId.PktEchoResult, pktResult);
    }
}

