using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Server;

using System.Text.Json;
using System.IO;

internal class GameServer : Server
{
    public override void Initialize(string configPath)
    {
        base.Initialize(configPath);

        Console.WriteLine("initialize server...");
    }
}