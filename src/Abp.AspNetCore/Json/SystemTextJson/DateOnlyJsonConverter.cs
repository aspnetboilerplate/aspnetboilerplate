using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abp.Json.SystemTextJson;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return DateOnly.MinValue;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var formats = new List<string>
            {
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss.fffffffzzz"
            };
            var str = reader.GetString();
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(str, format, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var outDateTime))
                {
                    return DateOnly.FromDateTime(outDateTime);
                }
            }
        }

        if (DateOnly.TryParse(reader.GetString(), CultureInfo.CurrentUICulture, out var date))
        {
            return date;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("O"));
    }
}