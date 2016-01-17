using System.Collections.Generic;
using Newtonsoft.Json;
using NSwag;
using NSwag.Collections;

namespace Abp.WebApi.Swagger.Adapter
{
    public class AbpSwaggerOperations : ObservableDictionary<SwaggerOperationMethod, AbpSwaggerOperation>
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperations"/> class.</summary>
        public AbpSwaggerOperations()
        {
            CollectionChanged += (sender, args) =>
            {
                foreach (var operation in Values)
                    operation.Parent = this; 
            };
        }

        /// <summary>Gets the parent <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public AbpSwaggerService Parent { get; internal set; }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<AbpSwaggerParameter> Parameters { get; set; }
    }
}
