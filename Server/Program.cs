using System;

internal class Program
{
    private static void Main(string[] args)
    {
        Acceptor acceptor = new Acceptor();
        acceptor.Initialize();

        acceptor.Run();

        Console.ReadLine();
    }
}