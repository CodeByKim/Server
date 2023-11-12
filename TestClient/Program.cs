using Core.Connection;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    static async Task Main(string[] args)
    {
        var ip = "127.0.0.1";
        var portNumber = 8888;

        var connector = new Connector();
        connector.Initialize();
        connector.OnDisconnectedHandler += (reason) =>
        {
            Logger.Info($"Disconnected... Reason: {reason}");
        };

        await connector.ConnectAsync(ip, portNumber);
        Logger.Info("Success Connect");

        while (true)
        {
            connector.Send("Hello World");

            Thread.Sleep(1000);
        }

        Console.ReadLine();
    }
}