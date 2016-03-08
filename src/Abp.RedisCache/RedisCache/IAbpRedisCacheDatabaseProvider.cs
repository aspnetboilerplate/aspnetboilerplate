using StackExchange.Redis;

namespace Abp.RedisCache
{
    /// <summary>
    /// Used to get <see cref="IDatabase"/> for Redis cache.
    /// </summary>
    public interface IAbpRedisCacheDatabaseProvider 
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        IDatabase GetDatabase();
    }
}
