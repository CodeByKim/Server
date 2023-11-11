using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Connection;
using Core.Server;
using Core.Util;

internal class GameServer : BaseServer<GameConnection>
{
    public override void Initialize(string configPath)
    {
        base.Initialize(configPath);

        Logger.Info("initialize server...");
    }

    protected override void OnNewConnection(GameConnection conn)
    {
        Logger.Info($"OnNewConnection: {conn.GetHashCode()}");
    }

    protected override void OnDisconnected(GameConnection conn, DisconnectReason reason)
    {
        Logger.Info($"OnDisconnected: {conn.GetHashCode()}, Reason: {reason.ToString()}");
    }
}