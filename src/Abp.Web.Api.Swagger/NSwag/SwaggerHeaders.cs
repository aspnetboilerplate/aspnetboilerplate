//-----------------------------------------------------------------------
// <copyright file="SwaggerHeaders.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>A collection of headers.</summary>
    public class SwaggerHeaders : ObservableDictionary<string, JsonSchema4>
    {
    }
}