using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abp.Json.SystemTextJson;

public class CultureInvariantDoubleJsonConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
            {
                return result;
            }
        }

        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public class CultureInvariantNullableDoubleJsonConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
            {
                return result;
            }
        }

        if (reader.TryGetDouble(out var d))
        {
            return d;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}