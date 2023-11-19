using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Connection;
using Core.Packet;
using Core.Server;
using Core.Util;

internal class GameServer : BaseServer<GameConnection>
{
    public GameServer(string configPath) : base(configPath)
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        Logger.Info("initialize server...");
    }

    protected override AbstractPacketResolver<GameConnection> OnRegisterPacketResolver()
    {
        return new GamePacketResolver();
    }

    protected override void OnNewConnection(GameConnection conn)
    {
        Logger.Info($"OnNewConnection: {conn.ID}");
    }

    protected override void OnDisconnected(GameConnection conn, DisconnectReason reason)
    {
        Logger.Info($"OnDisconnected: {conn.ID}, Reason: {reason}");
    }
}