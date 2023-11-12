using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Server
{
    public class ServerConfig
    {
        public int PortNumber { get; set; } = 10000;

        public int Backlog { get; set; } = 10;

        public int ConnectionPoolCount { get; set; } = 100;

        private static ServerConfig _instance;

        public static ServerConfig Instance => _instance;

        public static void Load(string path)
        {
            var rawData = File.ReadAllText(path);
            _instance = JsonSerializer.Deserialize<ServerConfig>(rawData);
        }
    }
}
