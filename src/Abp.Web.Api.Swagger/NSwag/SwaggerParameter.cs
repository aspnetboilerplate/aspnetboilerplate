//-----------------------------------------------------------------------
// <copyright file="SwaggerParameter.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;

namespace NSwag
{
    /// <summary>Describes an operation parameter. </summary>
    public class SwaggerParameter : JsonSchema4 
    {
        // TODO: Only some properties of JsonSchema4 are allowed

        /// <summary>Gets or sets the name.</summary>
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name { get; set; }

        /// <summary>Gets or sets the kind of the parameter.</summary>
        [JsonProperty(PropertyName = "in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SwaggerParameterKind Kind { get; set; }

        /// <summary>Gets or sets a value indicating whether the parameter is required.</summary>
        [JsonIgnore]
        public bool IsRequired
        {
            get { return IsRequiredRaw.HasValue ? IsRequiredRaw.Value : true; }
            set { IsRequiredRaw = value; }
        }

        /// <summary>Gets or sets the raw required value.</summary>
        [JsonProperty(PropertyName = "isRequired", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal bool? IsRequiredRaw { get; set; }

        /// <summary>Gets or sets the schema which is only available when <see cref="Kind"/> == body.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JsonSchema4 Schema { get; set; }

        /// <summary>Gets the actual schema, either the parameter schema itself (or its reference) or the <see cref="Schema"/> property when <see cref="Kind"/> == body.</summary>
        /// <exception cref="InvalidOperationException" accessor="get">The schema reference path is not resolved.</exception>
        [JsonIgnore]
        public override JsonSchema4 ActualSchema
        {
            get { return Kind == SwaggerParameterKind.body ? Schema.ActualSchema : base.ActualSchema; }
        }
    }
}