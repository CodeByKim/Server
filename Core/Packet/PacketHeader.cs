using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace Core.Packet
{
    internal struct PacketHeader
    {
        public static readonly short HeaderSize = 4;

        public short PacketId { get; private set; }
        public short Payload { get; private set; }

        public PacketHeader(IMessage packet)
        {
            PacketId = 0;
            Payload = (short)packet.CalculateSize();
        }
    }
}
