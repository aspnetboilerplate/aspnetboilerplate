using System;
using System.IO;
using ProtoBuf;
using StackExchange.Redis;

namespace Abp.Runtime.Caching.Redis
{
    public class ProtoBufRedisCacheSerializer : IRedisCacheSerialization
    {
        private const string TypeSeperator = "|";

        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="objbyte">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="IRedisCacheSerialization.Serialize" />
        public object Deserialize(RedisValue objbyte)
        {
            string serializedObj = objbyte;

            int typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator);
            Type type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
            string serialized = serializedObj.Substring(typeSeperatorIndex + 1);

            byte[] byteAfter64 = Convert.FromBase64String(serialized);
            MemoryStream afterStream = new MemoryStream(byteAfter64);

            return Serializer.Deserialize(type, afterStream);

            //MethodInfo method = typeof(Serializer).GetMethod(nameof(Serializer.Deserialize));
            //MethodInfo generic = method.MakeGenericMethod(type);
            //return generic.Invoke(null, new object[]{afterStream});
        }

        /// <summary>
        ///     Produce a string representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Returns a string representing the object instance that can be placed into the Redis cache.</returns>
        /// <seealso cref="IRedisCacheSerialization.Deserialize" />
        public string Serialize(object value, Type type)
        {
            MemoryStream msTestString = new MemoryStream();
            Serializer.Serialize(msTestString, value);
            string serialized = Convert.ToBase64String(msTestString.GetBuffer(), 0, (int) msTestString.Length);

            return $"{type.AssemblyQualifiedName}{TypeSeperator}{serialized}";
        }
    }
}