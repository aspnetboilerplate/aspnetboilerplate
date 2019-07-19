using System;
using System.IO;
using System.Reflection;
using ProtoBuf;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    public class ProtoBufRedisCacheSerializer : DefaultRedisCacheSerializer
    {
        private const string ProtoBufPrefix = "PB^";

        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="redisValue">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="IRedisCacheSerializer.Serialize" />
        public override T Deserialize<T>(RedisValue redisValue)
        {
            string serializedObj = redisValue;
            if (!serializedObj.StartsWith(ProtoBufPrefix))
            {
                return base.Deserialize<T>(redisValue);
            }

            serializedObj = serializedObj.Substring(ProtoBufPrefix.Length);
            var protoBufPrefixIndex = serializedObj.IndexOf(ProtoBufPrefix, StringComparison.OrdinalIgnoreCase);
            var serialized = serializedObj.Substring(protoBufPrefixIndex + 1);
            var byteAfter64 = Convert.FromBase64String(serialized);

            using (var memoryStream = new MemoryStream(byteAfter64))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        /// <summary>
        ///     Produce a string representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <returns>Returns a string representing the object instance that can be placed into the Redis cache.</returns>
        /// <seealso cref="IRedisCacheSerializer.Deserialize" />
        public override string Serialize(object value)
        {
            if (!value.GetType().GetTypeInfo().IsDefined(typeof(ProtoContractAttribute), false))
            {
                return base.Serialize(value);
            }

            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, value);
                var byteArray = memoryStream.ToArray();
                var serialized = Convert.ToBase64String(byteArray, 0, byteArray.Length);
                return $"{ProtoBufPrefix}{serialized}";
            }
        }
    }
}