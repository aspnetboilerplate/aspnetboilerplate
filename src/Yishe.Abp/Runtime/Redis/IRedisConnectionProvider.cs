using Abp.Dependency;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Runtime.Redis
{
    public interface IRedisConnectionProvider : ISingletonDependency
    {
        ConnectionMultiplexer GetConnection(string connectionString);
        string GetConnectionString(string service);
    }
}
