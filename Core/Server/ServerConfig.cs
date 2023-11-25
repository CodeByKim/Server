using Core.Util;
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

        public int ReceiveBufferSize { get; set; } = 1024 * 4;

        public int RoomCount { get; set; } = 1;

        private static ServerConfig _instance = new ServerConfig();
        public static ServerConfig Instance => _instance;

        public static void Load(string path)
        {
            try
            {
                var rawData = File.ReadAllText(path);
                _instance = JsonSerializer.Deserialize<ServerConfig>(rawData);
            }
            catch (Exception e)
            {
                Logger.Error($"Fail Loading Config... Message: {e.Message}, Path: {Path.GetFullPath(path)}");
                Environment.Exit(0);
            }
        }
    }
}
