using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Google.Protobuf;

namespace Core.Packet
{
    internal static class PacketUtil
    {
        internal static byte[] CreateBuffer(in PacketHeader header, IMessage packet)
        {
            var buffer = new byte[PacketHeader.HeaderSize + header.Payload];
            HeaderToBuffer(header, buffer);
            PacketToBuffer(packet, header.Payload, buffer);

            return buffer;
        }

        private static void HeaderToBuffer(in PacketHeader header, byte[] buffer)
        {
            var packetId = BitConverter.GetBytes(header.PacketId);
            var payload = BitConverter.GetBytes(header.Payload);

            Array.Copy(packetId, 0, buffer, 0, sizeof(short));
            Array.Copy(payload, sizeof(short), buffer, sizeof(short), sizeof(short));
        }

        private static void PacketToBuffer(IMessage packet, short payload, byte[] buffer)
        {
            Array.Copy(packet.ToByteArray(), 0, buffer, PacketHeader.HeaderSize, payload);
        }
    }
}
