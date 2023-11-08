using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server
{
    internal class ServerConfig
    {
        public int PortNumber { get; set; } = 10000;

        public int Backlog { get; set; } = 10;
    }
}
