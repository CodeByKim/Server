using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packet
{
    internal struct PacketHeader
    {
        public static readonly short HeaderSize = 4;

        public short Payload { get; set; }
        public short PacketId { get; set; }

        public PacketHeader()
        {
        }
    }
}
