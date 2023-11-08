using System;

internal class Program
{
    private static void Main(string[] args)
    {
        GameServer server = new GameServer();
        server.Initialize();

        server.Run();

        Console.ReadLine();
    }
}