//-----------------------------------------------------------------------
// <copyright file="SwaggerOperations.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>A Swagger path.</summary>
    public class SwaggerOperations : ObservableDictionary<SwaggerOperationMethod, SwaggerOperation>
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperations"/> class.</summary>
        public SwaggerOperations()
        {
            CollectionChanged += (sender, args) =>
            {
                foreach (var operation in Values)
                    operation.Parent = this; 
            };
        }

        /// <summary>Gets the parent <see cref="SwaggerService"/>.</summary>
        [JsonIgnore]
        public SwaggerService Parent { get; internal set; }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerParameter> Parameters { get; set; }
    }
}