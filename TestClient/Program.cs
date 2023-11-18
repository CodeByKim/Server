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

        var connector = new Connector();
        connector.Initialize();

        connector.OnDisconnectedHandler += (reason) =>
        {
            Logger.Info($"Disconnected... Reason: {reason}");
        };

        connector.OnGetPacketIdHandler += (packet) =>
        {
            var packetName = packet.Descriptor.Name;
            var packetId = Enum.Parse<PacketId>(packetName);

            return (short)packetId;
        };

        await connector.ConnectAsync(ip, portNumber);
        Logger.Info("Success Connect");

        while (true)
        {
            PktEcho pkt = new PktEcho();
            pkt.Message = "Echo Test";
            connector.Send(pkt);

            //connector.Send("Hello World");
            Thread.Sleep(1000);
        }

        Console.ReadLine();
    }
}