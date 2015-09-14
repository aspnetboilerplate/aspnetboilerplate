using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RedisCache
{
    public interface IAbpRedisConnectionProvider 
    {
        ConnectionMultiplexer GetConnection(string connectionString);
        string GetConnectionString(string service);
    }
}
