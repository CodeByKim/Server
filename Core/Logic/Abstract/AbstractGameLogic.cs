using Core.Connection;
using Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logic
{
    internal abstract class AbstractGameLogic<TConnection> : AbstractLogic<TConnection>
        where TConnection : ClientConnection<TConnection>, new()
    {
        public AbstractGameLogic(BaseServer<TConnection> server) : base(server)
        {
        }
    }
}
