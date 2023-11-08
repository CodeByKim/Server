using System;

internal class Program
{
    private static void Main(string[] args)
    {
        GameServer server = new GameServer();
        server.Initialize("config.json");

        server.Run();

        Console.ReadLine();
    }
}