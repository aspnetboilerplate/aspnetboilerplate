using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Reflection;
using Abp.Timing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Abp.Json
{
    public class AbpDateTimeConverter : DateTimeConverterBase, ITransientDependency
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        private readonly DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        protected List<string> InputDateTimeFormats { get; set; }
        protected string OutputDateTimeFormat { get; set; }

        public AbpDateTimeConverter(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
        {
            InputDateTimeFormats = inputDateTimeFormats ?? new List<string>();
            OutputDateTimeFormat = outputDateTimeFormat;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var nullable = Nullable.GetUnderlyingType(objectType) != null;
            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable)
                {
                    throw new JsonSerializationException($"Cannot convert null value to {objectType.FullName}.");
                }

                return null;
            }

            if (reader.TokenType == JsonToken.Date)
            {
                return Clock.Normalize(reader.Value.To<DateTime>());
            }

            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonSerializationException($"Unexpected token parsing date. Expected String, got {reader.TokenType}.");
            }

            var dateText = reader.Value?.ToString();

            if (dateText.IsNullOrEmpty() && nullable)
            {
                return null;
            }

            if (InputDateTimeFormats.Any())
            {
                foreach (var format in InputDateTimeFormats)
                {
                    if (DateTime.TryParseExact(dateText, format, _culture, _dateTimeStyles, out var d1))
                    {
                        return Clock.Normalize(d1);
                    }
                }
            }

            var date = DateTime.Parse(dateText, _culture, _dateTimeStyles);

            return Clock.Normalize(date);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                value = Clock.Normalize(value.To<DateTime>());
            }

            if (value is DateTime dateTime)
            {
                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal ||
                    (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTime = dateTime.ToUniversalTime();
                }

                writer.WriteValue(OutputDateTimeFormat.IsNullOrWhiteSpace()
                    ? dateTime.ToString(DefaultDateTimeFormat, _culture)
                    : dateTime.ToString(OutputDateTimeFormat, _culture));
            }
            else
            {
                throw new JsonSerializationException($"Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {value.GetType()}.");
            }
        }

        internal static bool ShouldNormalize(MemberInfo member, JsonProperty property)
        {
            if (property.PropertyType != typeof(DateTime) &&
                property.PropertyType != typeof(DateTime?))
            {
                return false;
            }

            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(member) == null;
        }
    }
}
