//-----------------------------------------------------------------------
// <copyright file="SwaggerParameterKind.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag
{
    /// <summary>Enumeration of the parameter kinds. </summary>
    public enum SwaggerParameterKind
    {
        /// <summary>An undefined kind.</summary>
        Undefined, 

        /// <summary>A JSON object as POST or PUT body (only one parameter of this type is allowed). </summary>
        body,

        /// <summary>A query key-value pair. </summary>
        query,

        /// <summary>An URL path placeholder. </summary>
        path,

        /// <summary>A HTTP header parameter.</summary>
        header,

        /// <summary>A form data parameter.</summary>
        formData
    }
}