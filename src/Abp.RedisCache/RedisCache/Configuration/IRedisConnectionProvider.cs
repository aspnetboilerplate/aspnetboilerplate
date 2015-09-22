using StackExchange.Redis;

namespace Abp.RedisCache.Configuration
{
    public interface IAbpRedisConnectionProvider 
    {
        ConnectionMultiplexer GetConnection(string connectionString);

        string GetConnectionString(string service);
    }
}
