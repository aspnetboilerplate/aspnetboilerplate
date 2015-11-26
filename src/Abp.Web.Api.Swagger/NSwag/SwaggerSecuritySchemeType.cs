//-----------------------------------------------------------------------
// <copyright file="SwaggerSecuritySchemeType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary></summary>
    public enum SwaggerSecuritySchemeType
    {
        /// <summary>The security scheme is not defined.</summary>
        Undefined,

        /// <summary>Basic authentication.</summary>
        basic,

        /// <summary>API key authentication.</summary>
        apiKey,

        /// <summary>OAuth2 authentication.</summary>
        oauth2
    }
}