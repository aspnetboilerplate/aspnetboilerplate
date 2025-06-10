using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Abp.Reflection;
using Abp.Timing;

namespace Abp.Json.SystemTextJson
{
    public class AbpDateTimeConverterModifier
    {
        private readonly List<string> _inputDateTimeFormats;
        private readonly string _outputDateTimeFormat;

        public AbpDateTimeConverterModifier(List<string> inputDateTimeFormats, string outputDateTimeFormat)
        {
            _inputDateTimeFormats = inputDateTimeFormats;
            _outputDateTimeFormat = outputDateTimeFormat;
        }

        public Action<JsonTypeInfo> CreateModifyAction()
        {
            return Modify;
        }

        private void Modify(JsonTypeInfo jsonTypeInfo)
        {
            if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(jsonTypeInfo.Type) != null)
            {
                return;
            }

            foreach (var property in jsonTypeInfo.Properties.Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?)))
            {
                if (property.AttributeProvider == null ||
                    !property.AttributeProvider.GetCustomAttributes(typeof(DisableDateTimeNormalizationAttribute), false).Any())
                {
                    property.CustomConverter = property.PropertyType == typeof(DateTime)
                        ? (JsonConverter) new AbpDateTimeConverter(_inputDateTimeFormats, _outputDateTimeFormat)
                        : new AbpNullableDateTimeConverter(_inputDateTimeFormats, _outputDateTimeFormat);
                }
            }
        }
    }
}
