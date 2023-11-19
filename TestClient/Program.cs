using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Core.Connection;
using Core.Util;

using Google.Protobuf;
using Protocol;

internal class Program
{
    static async Task Main(string[] args)
    {
        var ip = "127.0.0.1";
        var portNumber = 8888;

        var connector = new DummyConnector();

        await connector.ConnectAsync(ip, portNumber);
        Logger.Info("Success Connect");


        while (true)
        {
            PktEcho pkt = new PktEcho();
            pkt.Message = "Echo Test";

            connector.Send((short)PacketId.PktEcho, pkt);

            Thread.Sleep(10);
        }

        Console.ReadLine();
    }
}