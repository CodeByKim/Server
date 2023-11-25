using Core.Connection;
using Core.Server;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logic
{
    internal abstract class AbstractLogic<TConnection>
        where TConnection : ClientConnection<TConnection>, new()
    {
        protected BaseServer<TConnection> _server;

        public AbstractLogic(BaseServer<TConnection> server)
        {
            _server = server;
        }

        internal void Run()
        {
            OnInitialize();

            ThreadPool.QueueUserWorkItem(
                (param) =>
                {
                    while (true)
                    {
                        OnUpdate();

                        Thread.Sleep(10);
                    }
                });
        }

        public abstract void OnInitialize();

        public abstract void OnUpdate();
    }
}
