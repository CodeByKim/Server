using System;
using System.Collections.Generic;
using Core.Buffer;
using Google.Protobuf;

namespace Core.Packet
{
    public struct PacketHeader
    {
        public static readonly short HeaderSize = 4;

        public short PacketId { get; private set; }
        public short Payload { get; private set; }

        public PacketHeader(IMessage packet, short packetId)
        {
            PacketId = packetId;
            Payload = (short)packet.CalculateSize();
        }

        internal void CopyTo(RingBuffer buffer)
        {
            var data = buffer.Peek(HeaderSize);

            PacketId = BitConverter.ToInt16(data.Array, data.Offset);
            Payload = BitConverter.ToInt16(data.Array, data.Offset + sizeof(short));
        }
    }
}
