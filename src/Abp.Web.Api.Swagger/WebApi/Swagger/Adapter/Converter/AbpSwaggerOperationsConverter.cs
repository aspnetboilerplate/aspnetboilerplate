using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Abp.WebApi.Swagger.Adapter.Converter
{
    internal class AbpSwaggerOperationsConverter : JsonConverter
    {
        private readonly Type[] _types;
        public AbpSwaggerOperationsConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var pair = (AbpSwaggerOperations)value;

            writer.WriteStartObject();
            var count = pair.Keys.Count;
            for (var i = 0; i < count; i++)
            {
                var key = pair.Keys.ElementAt(i);
                var oVal = pair.Values.ElementAt(i);
                
                writer.WritePropertyName(key.ToString().ToLower());

                var tokenReader = new JTokenReader(JToken.FromObject(oVal));
                writer.WriteToken(tokenReader);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }

        public override bool CanRead => false;
    }
}
