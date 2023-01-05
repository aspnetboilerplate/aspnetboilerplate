using System;
using Abp.Dependency;
using Abp.Timing;
using Newtonsoft.Json;

namespace Abp.Json
{
    /// <summary>
    /// Defines helper methods to work with JSON.
    /// </summary>
    public class JsonSerializationHelper: ITransientDependency
    {
        private const char TypeSeperator = '|';

        private readonly IClock _clock;

        public JsonSerializationHelper(IClock clock)
        {
            _clock = clock;
        }

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        public static string SerializeWithType(object obj)
        {
            return SerializeWithType(obj, obj.GetType());
        }

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        public static string SerializeWithType(object obj, Type type)
        {
            var serialized = obj.ToJsonString();

            return string.Format(
                "{0}{1}{2}",
                type.AssemblyQualifiedName,
                TypeSeperator,
                serialized
                );
        }

        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        public T DeserializeWithType<T>(string serializedObj)
        {
            return (T)DeserializeWithType(serializedObj);
        }

        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        public object DeserializeWithType(string serializedObj)
        {
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator);
            var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);

            var options = new JsonSerializerSettings
            {
                ContractResolver = new AbpCamelCasePropertyNamesContractResolver(_clock)
            };

            return JsonConvert.DeserializeObject(serialized, type, options);
        }
    }
}