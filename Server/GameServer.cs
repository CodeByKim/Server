using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Server;

internal class GameServer : BaseServer
{
    public override void Initialize(string configPath)
    {
        base.Initialize(configPath);

        Console.WriteLine("initialize server...");
    }
}