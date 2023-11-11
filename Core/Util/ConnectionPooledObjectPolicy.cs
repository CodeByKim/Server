using System;
using Microsoft.Extensions.ObjectPool;

using Core.Connection;
using System.Data.Common;

namespace Core.Util
{
    public class ConnectionPooledObjectPolicy<TConnection> : IPooledObjectPolicy<TConnection>
        where TConnection : BaseConnection<TConnection>, new()
    {
        public TConnection Create()
        {
            var connection = new TConnection();
            return connection;
        }

        public bool Return(TConnection conn)
        {
            conn.Release();
            return true;
        }
    }
}
