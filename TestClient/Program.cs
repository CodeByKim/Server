using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    static async Task Main(string[] args)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var port = 8888;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

        await socket.ConnectAsync(endPoint);

        Console.WriteLine("connect!");

        byte[] message = Encoding.UTF8.GetBytes("Hello World");

        while (true)
        {
            socket.Send(message);

            Thread.Sleep(1000);
        }

        Console.ReadLine();
    }
}