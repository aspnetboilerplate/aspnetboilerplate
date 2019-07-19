using Abp.Dependency;
using Abp.Json;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    ///     Default implementation uses JSON as the underlying persistence mechanism.
    /// </summary>
    public class DefaultRedisCacheSerializer : IRedisCacheSerializer, ITransientDependency
    {
        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="redisValue">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="IRedisCacheSerializer.Serialize" />
        public virtual T Deserialize<T>(RedisValue redisValue)
        {
            return redisValue.ToString().FromJsonString<T>();
        }

        /// <summary>
        ///     Produce a string representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <returns>Returns a string representing the object instance that can be placed into the Redis cache.</returns>
        /// <seealso cref="IRedisCacheSerializer.Deserialize{T}" />
        public virtual string Serialize(object value)
        {
            return value.ToJsonString();
        }
    }
}