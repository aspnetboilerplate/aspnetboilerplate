using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSwag;

namespace Abp.WebApi.Swagger.Adapter
{
    public class AbpSwaggerParameter : SwaggerParameter
    {
        /// <summary>Gets or sets the kind of the parameter.</summary>
        [JsonProperty(PropertyName = "in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public new AbpSwaggerParameterKind Kind { get; set; }
    }
}
