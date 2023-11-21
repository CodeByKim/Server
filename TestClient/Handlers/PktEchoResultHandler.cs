using System;
using System.Collections.Generic;

using Core.Packet;
using Core.Util;
using Google.Protobuf;
using Protocol;

public class PktEchoResultHandler : AbstractPacketHandler<DummyConnector>
{
    public override void OnHandle(DummyConnector conn, IMessage packet)
    {
        var pkt = packet as PktEchoResult;

        Logger.Info($"From: {conn.ID}, message: {pkt.Message}");
    }
}