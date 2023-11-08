using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Server;

using System.Text.Json;
using System.IO;

internal class ServerConfig
{
    public int PortNumber { get; set; }
    public int Backlog { get; set; }
}

internal class GameServer : Server
{
    public override void Initialize()
    {
        base.Initialize();

        Console.WriteLine("initialize server...");

        var rawConfig = File.ReadAllText("config.json");
        var config = JsonSerializer.Deserialize<ServerConfig>(rawConfig);

        Console.WriteLine(config.PortNumber);
        Console.WriteLine(config.Backlog);
    }
}