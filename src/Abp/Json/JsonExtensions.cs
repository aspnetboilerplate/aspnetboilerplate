using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abp.Json.SystemTextJson;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Abp.Json
{
    public static class JsonExtensions
    {
        public static bool UseNewtonsoft { get; set; }

        private static readonly AbpCamelCasePropertyNamesContractResolver
            SharedAbpCamelCasePropertyNamesContractResolver;

        private static readonly AbpContractResolver SharedAbpContractResolver;
        private static readonly ConcurrentDictionary<object, JsonSerializerOptions> JsonSerializerOptionsCache;

        static JsonExtensions()
        {
            SharedAbpCamelCasePropertyNamesContractResolver = new AbpCamelCasePropertyNamesContractResolver();
            SharedAbpContractResolver = new AbpContractResolver();
            JsonSerializerOptionsCache = new ConcurrentDictionary<object, JsonSerializerOptions>();
            UseNewtonsoft = false;
        }

        /// <summary>
        /// Converts given object to JSON string.
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            return UseNewtonsoft
                ? ToJsonStringWithNewtonsoft(obj, camelCase, indented)
                : ToJsonStringWithSystemTextJson(obj, camelCase, indented);
        }

        /// <summary>
        /// Converts given object to JSON string.
        /// </summary>
        /// <returns></returns>
        private static string ToJsonStringWithNewtonsoft(this object obj, bool camelCase = false, bool indented = false)
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
        /// Converts given object to JSON string.
        /// </summary>
        /// <returns></returns>
        private static string ToJsonStringWithSystemTextJson(this object obj, bool camelCase = false,
            bool indented = false)
        {
            var options = CreateJsonSerializerOptions(camelCase, indented);
            return ToJsonString(obj, options);
        }

        public static JsonSerializerOptions CreateJsonSerializerOptions(bool camelCase = false, bool indented = false)
        {
            return JsonSerializerOptionsCache.GetOrAdd(new
            {
                camelCase,
                indented
            }, _ =>
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                options.Converters.Add(new AbpStringToEnumFactory());
                options.Converters.Add(new AbpStringToBooleanConverter());
                options.Converters.Add(new AbpStringToGuidConverter());
                options.Converters.Add(new AbpNullableStringToGuidConverter());
                options.Converters.Add(new AbpNullableFromEmptyStringConverterFactory());
                options.Converters.Add(new ObjectToInferredTypesConverter());
                options.Converters.Add(new AbpJsonConverterForType());

                options.TypeInfoResolver = new AbpDateTimeJsonTypeInfoResolver();

                if (camelCase)
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }

                if (indented)
                {
                    options.WriteIndented = true;
                }

                return options;
            });
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
        /// Converts given object to JSON string using custom <see cref="JsonSerializerOptions"/>.
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, JsonSerializerOptions options)
        {
            return obj != null
                ? JsonSerializer.Serialize(obj, options)
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
            return UseNewtonsoft
                ? value.FromJsonString<T>(new JsonSerializerSettings())
                : value.FromJsonString<T>(CreateJsonSerializerOptions());
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
        /// Returns deserialized string using custom <see cref="JsonSerializerOptions"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string value, JsonSerializerOptions options)
        {
            return value != null
                ? JsonSerializer.Deserialize<T>(value, options)
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

        /// <summary>
        /// Returns deserialized string using explicit <see cref="Type"/> and custom <see cref="JsonSerializerOptions"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object FromJsonString(this string value, [NotNull] Type type, JsonSerializerOptions options)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return value != null
                ? JsonSerializer.Deserialize(value, type, options)
                : null;
        }
    }
}