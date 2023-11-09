using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

using Core.Connection;

namespace Core.Server
{
    public abstract class BaseServer
    {
        internal static ServerConfig Config { get; private set; }

        private Acceptor _acceptor;

        public BaseServer()
        {
            _acceptor = new Acceptor();
        }

        public virtual void Initialize(string configPath)
        {
            LoadConfig(configPath);

            _acceptor.Initialize();
        }

        public void Run()
        {
            _acceptor.Run();
        }

        private void LoadConfig(string path)
        {
            var rawData = File.ReadAllText(path);
            Config = JsonSerializer.Deserialize<ServerConfig>(rawData);
        }
    }
}
