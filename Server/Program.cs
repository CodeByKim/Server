using System;

internal class Program
{
    private static void Main(string[] args)
    {
        var configPath = "config.json";

        var server = new GameServer(configPath);
        server.Initialize();
        server.Run();

        Console.ReadLine();
    }
}