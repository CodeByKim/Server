using Core.Connection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Server
{
    public abstract class Server
    {
        private Acceptor _acceptor;

        public Server()
        {
            _acceptor = new Acceptor();
        }

        public virtual void Initialize()
        {
            _acceptor.Initialize();
        }

        public void Run()
        {
            _acceptor.Run();
        }
    }
}
