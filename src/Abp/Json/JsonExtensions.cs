using Newtonsoft.Json;

namespace Abp.Json
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts given object to JSON string.
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            var options = new JsonSerializerSettings();

            if (camelCase)
            {
                options.ContractResolver = new AbpCamelCasePropertyNamesContractResolver();
            }
            else
            {
                options.ContractResolver = new AbpContractResolver();
            }

            if (indented)
            {
                options.Formatting = Formatting.Indented;
            }
            
            return JsonConvert.SerializeObject(obj, options);
        }
    }
}
