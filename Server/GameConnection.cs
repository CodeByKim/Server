using System;
using System.Collections.Generic;
using System.Text;

using Core.Connection;
using Core.Packet;
using Core.Util;
using Protocol;
using Google.Protobuf;

public class GameConnection : ClientConnection<GameConnection>
{
    public GameConnection() : base()
    {
    }

    protected override void OnParsePacket(PacketHeader header, ArraySegment<Byte> data)
    {
        //무조건 PktEcho라고 가정
        PktEcho pkt = new PktEcho();
        pkt.MergeFrom(data.Array, data.Offset, header.Payload);

        Logger.Info($"From: {ID}, message: {pkt.Message}");
    }
}
