using System;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    ///     Interface to be implemented by all custom (de)serialization methods used when persisting and retrieving
    ///     objects from the Redis cache.
    /// </summary>
    public interface IRedisCacheSerializer
    {
        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="redisValue">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="Serialize" />
        T Deserialize<T>(RedisValue redisValue);

        /// <summary>
        ///     Produce a string representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <returns>Returns a string representing the object instance that can be placed into the Redis cache.</returns>
        /// <seealso cref="Deserialize{T}" />
        string Serialize(object value);
    }
}