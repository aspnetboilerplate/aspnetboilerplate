using StackExchange.Redis;

namespace Abp.RealTime.Redis
{
    /// <summary>
    /// Used to get <see cref="IDatabase"/> for Redis cache.
    /// </summary>
    public interface IAbpRedisOnlineClientStoreDatabaseProvider 
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        IDatabase GetDatabase();
    }
}
