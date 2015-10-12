//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationMethod.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>Enumeration of the available HTTP methods. </summary>
    public enum SwaggerOperationMethod
    {
        /// <summary>The HTTP GET method. </summary>
        get,

        /// <summary>The HTTP POST method. </summary>
        post,

        /// <summary>The HTTP PUT method. </summary>
        put,

        /// <summary>The HTTP DELETE method. </summary>
        delete,

        /// <summary>The HTTP OPTIONS method. </summary>
        options,

        /// <summary>The HTTP HEAD method. </summary>
        head,

        /// <summary>The HTTP PATCH method. </summary>
        patch
    }
}