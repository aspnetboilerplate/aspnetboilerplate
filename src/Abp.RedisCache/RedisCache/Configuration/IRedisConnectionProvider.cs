using StackExchange.Redis;

namespace Abp.RedisCache.Configuration
{
    public interface IAbpRedisConnectionProvider 
    {
        ConnectionMultiplexer GetConnection(string connectionString);

        string GetConnectionString(string name);
        int GetDatabaseId(string name);
    }
}
