using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSwag;

namespace Abp.WebApi.Swagger.Adapter
{
    public class AbpSwaggerSecurityScheme : SwaggerSecurityScheme
    {
        /// <summary>Gets or sets the type of the security scheme.</summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public new AbpSwaggerSecuritySchemeType Type { get; set; }
    }
}
