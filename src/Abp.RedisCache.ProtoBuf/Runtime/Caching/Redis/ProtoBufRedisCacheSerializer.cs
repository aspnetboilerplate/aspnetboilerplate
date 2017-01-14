using System;
using System.IO;
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
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator, StringComparison.InvariantCultureIgnoreCase);
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
        public override string Serialize(object value, Type type)
        {
            if (!type.IsDefined(typeof(ProtoContractAttribute), false))
            {
                return base.Serialize(value, type);
            }

            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, value);
                var serialized = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                return $"{ProtoBufPrefix}{type.AssemblyQualifiedName}{TypeSeperator}{serialized}";
            }
        }
    }
}