using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Connection;
using Core.Server;

internal class GameServer : BaseServer
{
    public override void Initialize(string configPath)
    {
        base.Initialize(configPath);

        Console.WriteLine("initialize server...");
    }

    protected override void OnNewConnection(BaseConnection conn)
    {
        Console.WriteLine("OnNewConnection: " + conn.GetHashCode());
    }

    protected override void OnDisconnected(BaseConnection conn, DisconnectReason reason)
    {
        Console.WriteLine("OnDisconnected: " + conn.GetHashCode());
    }
}