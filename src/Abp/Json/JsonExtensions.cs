using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Abp.Json
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts given object to JSON string.
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            var options = CreateAbpSerializerSettings(camelCase, indented);
            return JsonConvert.SerializeObject(obj, options);
        }

        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Error)
        {
            var options = CreateAbpSerializerSettings(camelCase, indented);
            options.ReferenceLoopHandling = ReferenceLoopHandling.Error;
            return JsonConvert.SerializeObject(obj, options);
        }

        /// <summary>
        /// Returns deserialized string using default <see cref="JsonSerializerSettings"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string value)
        {
            return value.FromJsonString<T>(new JsonSerializerSettings());
        }

        /// <summary>
        /// Returns deserialized string using custom <see cref="JsonSerializerSettings"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string value, JsonSerializerSettings settings)
        {
            return value != null
                ? JsonConvert.DeserializeObject<T>(value, settings)
                : default(T);
        }

        /// <summary>
        /// Returns deserialized string using explicit <see cref="Type"/> and custom <see cref="JsonSerializerSettings"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static object FromJsonString(this string value, [NotNull] Type type, JsonSerializerSettings settings)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return value != null
                ? JsonConvert.DeserializeObject(value, type, settings)
                : null;
        }

        private static JsonSerializerSettings CreateAbpSerializerSettings(bool camelCase, bool indented)
        {
            var options = new JsonSerializerSettings();

            if (camelCase)
            {
                options.ContractResolver = new AbpCamelCasePropertyNamesContractResolver();
            }
            else
            {
                options.ContractResolver = new AbpContractResolver();
            }

            if (indented)
            {
                options.Formatting = Formatting.Indented;
            }

            return options;
        }
    }
}