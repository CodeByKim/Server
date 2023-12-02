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

        internal override void Initialize()
        {
            base.Initialize();

            // 테스트 코드
            // 나중에 지워야 함
            for (int i = 0; i <10; i++)
            {
                var count = i;
                PushJob(
                    TimeSpan.FromSeconds(i),
                    () =>
                    {
                        Logger.Info($"Run Task... {count}");
                    });
            }
        }

        internal void Add(TConnection conn)
        {
            _connectons.Add(conn);
        }

        internal void OnUpdate()
        {
            foreach (var conn in _connectons)
                conn.ConsumePacket();

            FlushJob();
        }
    }
}
