using Core.Connection;
using Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logic
{
    internal abstract class AbstractSystemLogic<TConnection> : AbstractLogic<TConnection>
        where TConnection : ClientConnection<TConnection>, new()
    {
        public AbstractSystemLogic(BaseServer<TConnection> server) : base(server)
        {
        }

        public abstract void OnNewConnection(TConnection conn);
    }
}
