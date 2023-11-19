using System;
using System.Collections.Generic;

using Core.Connection;
using Core.Packet;
using Core.Server;
using Core.Util;

public class DummyConnector : Connector<DummyConnector>
{
    public DummyConnector()
    {
    }

    protected override void OnDisconnected(BaseConnection conn, DisconnectReason reason)
    {
        Logger.Info($"OnDisconnected: {conn.ID}, Reason: {reason}");
    }

    protected override AbstractPacketResolver<DummyConnector> OnRegisterPacketResolver()
    {
        return new GamePacketResolver();
    }
}
