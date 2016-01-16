using System.Collections.Generic;
using Abp.WebApi.Swagger.Adapter.Converter;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NSwag.Collections;

namespace Abp.WebApi.Swagger.Adapter
{
    public class AbpSwaggerService : SwaggerService
    {
        public AbpSwaggerService()
        {
            Tags = new List<AbpSwaggerTag>();
            SecurityDefinitions = new Dictionary<string, AbpSwaggerSecurityScheme>();

            Paths = new ObservableDictionary<string, AbpSwaggerOperations>();
            Paths.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Paths.Values)
                    path.Parent = this;
            };
        }

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "tags", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public new List<AbpSwaggerTag> Tags { get; set; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "securityDefinitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new Dictionary<string, AbpSwaggerSecurityScheme> SecurityDefinitions { get; private set; }

        /// <summary>Gets or sets the operations.</summary>
        [JsonProperty(PropertyName = "paths", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public new ObservableDictionary<string, AbpSwaggerOperations> Paths { get; private set; }

        /// <summary>Gets or sets the parameters which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public new List<AbpSwaggerParameter> Parameters { get; set; }


        /// <summary>Converts the description object to JSON.</summary>
        /// <returns>The JSON string.</returns>
        public new string ToJson()
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };

            var swaggerOperationsConverter = new AbpSwaggerOperationsConverter(typeof(AbpSwaggerOperations));
            var jsonSchema4Converter = new JsonSchema4Converter(typeof (JsonSchema4));

            settings.Converters.Add(swaggerOperationsConverter);
            settings.Converters.Add(jsonSchema4Converter);

            GenerateOperationIds();

            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(this);
            JsonSchemaReferenceUtilities.UpdateSchemaReferences(this);

            var data = JsonConvert.SerializeObject(this, settings);

            return JsonSchemaReferenceUtilities.ConvertPropertyReferences(data);
        }
    }
}
