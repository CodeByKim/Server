using System;
using System.Collections.Generic;

using Core.Connection;
using Protocol;

public class GameConnection : ClientConnection<GameConnection>
{
    public GameConnection() : base()
    {
        OnGetPacketIdHandler += (packet) =>
        {
            var packetName = packet.Descriptor.Name;
            var packetId = Enum.Parse<PacketId>(packetName);

            return (short)packetId;
        };
    }
}
