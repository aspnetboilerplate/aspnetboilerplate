//-----------------------------------------------------------------------
// <copyright file="SwaggerSecurityScheme.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag
{
    /// <summary>The definition of a security scheme that can be used by the operations.</summary>
    public class SwaggerSecurityScheme
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerSecurityScheme"/> class.</summary>
        public SwaggerSecurityScheme()
        {
            Scopes = new Dictionary<string, string>();   
        }

        /// <summary>Gets or sets the type of the security scheme.</summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SwaggerSecuritySchemeType Type { get; set; }

        /// <summary>Gets or sets the short description for security scheme.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the name of the header or query parameter to be used.</summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name { get; set; }

        /// <summary>Gets or sets the location of the API key.</summary>
        [JsonProperty(PropertyName = "in", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ApiKeyLocation { get; set; }

        /// <summary>Gets or sets the used by the OAuth2 security scheme.</summary>
        [JsonProperty(PropertyName = "flow", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Flow { get; set; }

        /// <summary>Gets or sets the authorization URL to be used for this flow.</summary>
        [JsonProperty(PropertyName = "authorizationUrl", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string AuthorizationUrl { get; set; }

        /// <summary>Gets or sets the token URL to be used for this flow. .</summary>
        [JsonProperty(PropertyName = "tokenUrl", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string TokenUrl { get; set; }

        /// <summary>Gets the available scopes for the OAuth2 security scheme.</summary>
        [JsonProperty(PropertyName = "scopes", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Dictionary<string, string> Scopes { get; private set; }
    }
}