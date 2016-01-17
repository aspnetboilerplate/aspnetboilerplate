using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NSwag;

namespace Abp.WebApi.Swagger.Adapter
{
    public class AbpSwaggerOperation : SwaggerOperation
    {
        public AbpSwaggerOperation()
        {
            Parameters = new List<AbpSwaggerParameter>();
        }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new List<AbpSwaggerParameter> Parameters { get; set; }

        /// <summary>Gets the parent operations list.</summary>
        [JsonIgnore]
        public new AbpSwaggerOperations Parent { get; internal set; }

        /// <summary>Gets the parameters from the operation and from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public new IEnumerable<AbpSwaggerParameter> AllParameters
        {
            get
            {
                var empty = new List<AbpSwaggerParameter>();
                return (Parameters ?? empty).Concat(Parent.Parameters ?? empty).Concat(Parent.Parent.Parameters ?? empty);
            }
        }
    }
}
