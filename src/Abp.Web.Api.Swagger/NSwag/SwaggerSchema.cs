//-----------------------------------------------------------------------
// <copyright file="SwaggerSchema.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>The enumeration of Swagger protocol schemes.</summary>
    public enum SwaggerSchema
    {
        /// <summary>The HTTP schema.</summary>
        http,

        /// <summary>The HTTPS schema.</summary>
        https,

        /// <summary>The WS schema.</summary>
        ws,

        /// <summary>The WSS schema.</summary>
        wss
    }
}