using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
            _acceptor = new Acceptor(this);
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

        internal void AcceptNewClient(Socket socket)
        {
            var conn = new BaseConnection(socket, this);
            OnNewConnection(conn);

            conn.ReceiveAsync();
        }

        internal void Disconnect(BaseConnection conn)
        {
            OnDisconnected(conn);
        }

        private void LoadConfig(string path)
        {
            var rawData = File.ReadAllText(path);
            Config = JsonSerializer.Deserialize<ServerConfig>(rawData);
        }

        protected abstract void OnNewConnection(BaseConnection conn);

        protected abstract void OnDisconnected(BaseConnection conn);
    }
}
