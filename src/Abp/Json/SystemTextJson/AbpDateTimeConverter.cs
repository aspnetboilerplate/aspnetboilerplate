using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Timing;

namespace Abp.Json.SystemTextJson
{
    public class AbpDateTimeConverter : JsonConverter<DateTime>
    {
        protected List<string> InputDateTimeFormats { get; set; }
        protected string OutputDateTimeFormat { get; set; }

        public AbpDateTimeConverter(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
        {
            InputDateTimeFormats = inputDateTimeFormats ?? new List<string>();
            OutputDateTimeFormat = outputDateTimeFormat;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!InputDateTimeFormats.IsNullOrEmpty())
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var s = reader.GetString();
                    foreach (var format in InputDateTimeFormats)
                    {
                        if (DateTime.TryParseExact(s, format, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var outDateTime))
                        {
                            return Clock.Normalize(outDateTime);
                        }
                    }
                }
                else
                {
                    throw new JsonException("Reader's TokenType is not String!");
                }
            }

            var dateText = reader.GetString();
            if (DateTime.TryParse(dateText, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var date))
            {
                return Clock.Normalize(date);    
            }

            throw new JsonException("Can't get datetime from the reader!");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (OutputDateTimeFormat.IsNullOrWhiteSpace())
            {
                writer.WriteStringValue(Clock.Normalize(value));
            }
            else
            {
                writer.WriteStringValue(Clock.Normalize(value).ToString(OutputDateTimeFormat, CultureInfo.CurrentUICulture));
            }
        }
    }

    public class AbpNullableDateTimeConverter : JsonConverter<DateTime?>, ITransientDependency
    {
        protected List<string> InputDateTimeFormats { get; set; }
        protected string OutputDateTimeFormat { get; set; }

        public AbpNullableDateTimeConverter(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
        {
            InputDateTimeFormats = inputDateTimeFormats ?? new List<string>();
            OutputDateTimeFormat = outputDateTimeFormat;
        }
        
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!InputDateTimeFormats.IsNullOrEmpty())
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var s = reader.GetString();
                    if (s.IsNullOrEmpty())
                    {
                        return null;
                    }
                    
                    foreach (var format in InputDateTimeFormats)
                    {
                        if (DateTime.TryParseExact(s, format, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var outDateTime))
                        {
                            return Clock.Normalize(outDateTime);
                        }
                    }
                }
                else
                {
                    throw new JsonException("Reader's TokenType is not String!");
                }
            }

            var dateText = reader.GetString();
            if (dateText.IsNullOrEmpty())
            {
                return null;
            }

            if (DateTime.TryParse(dateText, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var date))
            {
                return Clock.Normalize(date);    
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                if (OutputDateTimeFormat.IsNullOrWhiteSpace())
                {
                    writer.WriteStringValue(Clock.Normalize(value.Value));
                }
                else
                {
                    writer.WriteStringValue(Clock.Normalize(value.Value).ToString(OutputDateTimeFormat, CultureInfo.CurrentUICulture));
                }
            }
        }
    }
}
