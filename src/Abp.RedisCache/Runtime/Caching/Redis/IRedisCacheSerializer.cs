using System;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    public interface IRedisCacheSerializer : IRedisCacheSerializer<object, RedisValue>
    {
    }

    /// <summary>
    ///     Interface to be implemented by all custom (de)serialization methods used when persisting and retrieving
    ///     objects from the Redis cache.
    /// </summary>
    public interface IRedisCacheSerializer<TSource, TDestination>
    {
        /// <summary>
        ///     Produce a Redis representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Returns object data in Redis representation that can be placed into the Redis cache.</returns>
        /// <seealso cref="Deserialize" />
        TDestination Serialize(TSource value, Type type);

        /// <summary>
        ///     Creates an instance of the object from object data in Redis representation.
        /// </summary>
        /// <param name="objbyte">Object data in Redis representation.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="Serialize" />
        TSource Deserialize(TDestination objbyte);
    }
}