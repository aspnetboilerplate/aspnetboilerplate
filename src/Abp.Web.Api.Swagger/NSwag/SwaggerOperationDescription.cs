//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationDescription.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>Flattened information about an operation.</summary>
    public class SwaggerOperationDescription
    {
        /// <summary>Gets the relative URL path.</summary>
        public string Path { get; internal set; }

        /// <summary>Gets the HTTP method.</summary>
        public SwaggerOperationMethod HttpMethod { get; internal set; }

        /// <summary>Gets the operation.</summary>
        public SwaggerOperation Operation { get; internal set; }
    }
}