using Core.Connection;
using Core.Job;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Logic
{
    internal class Room<TConnection> : JobExecutor
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

        internal void OnUpdate()
        {
            foreach (var conn in _connectons)
                conn.ConsumePacket();

            ExecuteJob();
        }
    }
}
