using Abp.Dependency;
using StackExchange.Redis;

namespace Abp.RedisCache.Configuration
{
    public interface IAbpRedisConnectionProvider:ISingletonDependency
    {
        ConnectionMultiplexer GetConnection(string connectionString);

        string GetConnectionString(string service);
    }
}
