using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using Abp.Timing;

namespace Abp.Json
{
    public static class JsonExtensions
    {
        private static readonly AbpCamelCasePropertyNamesContractResolver SharedAbpCamelCasePropertyNamesContractResolver;
        private static readonly AbpContractResolver SharedAbpContractResolver;

        static JsonExtensions()
        {
            //TODO@Clock -> Is this safe ? 
            var clock = new Clock();
            SharedAbpCamelCasePropertyNamesContractResolver = new AbpCamelCasePropertyNamesContractResolver(clock);
            SharedAbpContractResolver = new AbpContractResolver(clock);
        }

        /// <summary>
        /// Converts given object to JSON string.
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            var settings = new JsonSerializerSettings();

            if (camelCase)
            {
                settings.ContractResolver = SharedAbpCamelCasePropertyNamesContractResolver;
            }
            else
            {
                settings.ContractResolver = SharedAbpContractResolver;
            }

            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }
            
            return ToJsonString(obj, settings);
        }

        /// <summary>
        /// Converts given object to JSON string using custom <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, JsonSerializerSettings settings)
        {
            return obj != null
                ? JsonConvert.SerializeObject(obj, settings)
                : default(string);
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
    }
}