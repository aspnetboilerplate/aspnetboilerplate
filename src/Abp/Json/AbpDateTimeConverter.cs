using System;
using Abp.Timing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Abp.Json
{
    public class AbpDateTimeConverter : IsoDateTimeConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var date = base.ReadJson(reader, objectType, existingValue, serializer) as DateTime?;

            if (date.HasValue)
            {
                return Clock.Normalize(date.Value);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = value as DateTime?;
            base.WriteJson(writer, date.HasValue ? Clock.Normalize(date.Value) : value, serializer);
        }
    }
}