using System;
using System.IO;
using System.Reflection;
using ProtoBuf;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    public class ProtoBufRedisCacheSerializer : DefaultRedisCacheSerializer
    {
        private const string TypeSeperator = "|";
        private const string ProtoBufPrefix = "PB^";

        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="objbyte">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="IRedisCacheSerializer.Serialize" />
        public override object Deserialize(RedisValue objbyte)
        {
            string serializedObj = objbyte;
            if (!serializedObj.StartsWith(ProtoBufPrefix))
            {
                return base.Deserialize(objbyte);
            }

            serializedObj = serializedObj.Substring(ProtoBufPrefix.Length);
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator, StringComparison.OrdinalIgnoreCase);
            var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);
            var byteAfter64 = Convert.FromBase64String(serialized);

            using (var memoryStream = new MemoryStream(byteAfter64))
            {
                return Serializer.Deserialize(type, memoryStream);
            }
        }

        /// <summary>
        ///     Produce a string representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Returns a string representing the object instance that can be placed into the Redis cache.</returns>
        /// <seealso cref="IRedisCacheSerializer.Deserialize" />
        public override RedisValue Serialize(object value, Type type)
        {
            if (!type.GetTypeInfo().IsDefined(typeof(ProtoContractAttribute), false))
            {
                return base.Serialize(value, type);
            }

            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, value);
                var byteArray = memoryStream.ToArray();
                var serialized = Convert.ToBase64String(byteArray, 0, byteArray.Length);
                return $"{ProtoBufPrefix}{type.AssemblyQualifiedName}{TypeSeperator}{serialized}";
            }
        }
    }
}