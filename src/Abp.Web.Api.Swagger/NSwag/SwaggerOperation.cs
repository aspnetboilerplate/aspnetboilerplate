//-----------------------------------------------------------------------
// <copyright file="SwaggerOperation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag
{
    /// <summary>Describes a JSON web service operation. </summary>
    public class SwaggerOperation
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperations"/> class.</summary>
        public SwaggerOperation()
        {
            Tags = new List<string>();
            Parameters = new List<SwaggerParameter>();
            Responses = new Dictionary<string, SwaggerResponse>();
        }
        
        [JsonIgnore]
        public SwaggerOperations Parent { get; internal set; }

        /// <summary>Gets or sets the tags.</summary>
        [JsonProperty(PropertyName = "tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonProperty(PropertyName = "externalDocs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>Gets or sets the name of the operation.</summary>
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the operation ID (unique name).</summary>
        [JsonProperty(PropertyName = "operationId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OperationId { get; set; }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public List<SwaggerSchema> Schemes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonProperty(PropertyName = "consumes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Consumes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonProperty(PropertyName = "produces", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Produces { get; set; }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerParameter> Parameters { get; set; }

        /// <summary>Gets or sets the responses.</summary>
        [JsonProperty(PropertyName = "responses", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, SwaggerResponse> Responses { get; set; }

        /// <summary>Gets or sets a value indicating whether the operation is deprecated.</summary>
        [JsonProperty(PropertyName = "deprecated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Deprecated { get; set; }

        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerSecurityRequirement Security { get; set; }

        /// <summary>Gets the list of MIME types the operation can consume, either from the operation or from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public IEnumerable<string> ActualConsumes
        {
            get { return Consumes ?? Parent.Parent.Consumes; }
        }

        /// <summary>Gets the list of MIME types the operation can produce, either from the operation or from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public IEnumerable<string> ActualProduces
        {
            get { return Produces ?? Parent.Parent.Produces; }
        }

        /// <summary>Gets the actual schemes, either from the operation or from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public IEnumerable<SwaggerSchema> ActualSchemes
        {
            get { return Schemes ?? Parent.Parent.Schemes; }
        }

        /// <summary>Gets the parameters from the operation and from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public IEnumerable<SwaggerParameter> AllParameters
        {
            get
            {
                var empty = new List<SwaggerParameter>(); 
                return (Parameters ?? empty).Concat(Parent.Parameters ?? empty).Concat(Parent.Parent.Parameters ?? empty);
            }
        }

        /// <summary>Gets the responses from the operation and from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public IReadOnlyDictionary<string, SwaggerResponse> AllResponses
        {
            get
            {
                var empty = new Dictionary<string, SwaggerResponse>();
                return (Responses ?? empty).Concat(Parent.Parent.Responses ?? empty).ToDictionary(t => t.Key, t => t.Value);
            }
        }

        /// <summary>Gets the actual security description, either from the operation or from the <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public SwaggerSecurityRequirement ActualSecurity
        {
            get { return Security ?? Parent.Parent.Security; }
        }
    }
}