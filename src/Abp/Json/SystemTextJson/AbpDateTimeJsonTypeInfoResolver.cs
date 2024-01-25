using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;

namespace Abp.Json.SystemTextJson
{
    public class AbpDateTimeJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
    {
        public AbpDateTimeJsonTypeInfoResolver(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
        {
            Modifiers.Add(new AbpDateTimeConverterModifier(inputDateTimeFormats, outputDateTimeFormat).CreateModifyAction());
        }
    }
}
