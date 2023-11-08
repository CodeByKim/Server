using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Server;

internal class GameServer : Server
{
    public override void Initialize()
    {
        base.Initialize();

        Console.WriteLine("initialize server...");
    }
}