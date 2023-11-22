using Core.Connection;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Logic
{
    internal class Room<TConnection>
        where TConnection : ClientConnection<TConnection>, new()
    {
        private List<TConnection> _connectons;

        internal Room()
        {
            _connectons = new List<TConnection>();
        }

        internal void Add(TConnection conn)
        {
            _connectons.Add(conn);
        }

        internal void OnRun(object param)
        {
            while (true)
            {
                Update();

                Thread.Sleep(10);
            }
        }

        private void Update()
        {
            foreach (var conn in _connectons)
                conn.ConsumePacket();
        }
    }
}
