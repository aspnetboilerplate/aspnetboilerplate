using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.Json
{
    internal static class JsonHelper
    {
        public static string ConvertToJson(object obj, bool camelCase = false, bool indented = false)
        {
            var options = new JsonSerializerSettings();

            if (camelCase)
            {
                options.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (indented)
            {
                options.Formatting = Formatting.Indented;
            }

            return JsonConvert.SerializeObject(obj, options);
        }
    }
}
