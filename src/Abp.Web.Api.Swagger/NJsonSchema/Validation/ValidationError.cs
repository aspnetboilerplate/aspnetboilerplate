//-----------------------------------------------------------------------
// <copyright file="ValidationError.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NJsonSchema.Validation
{
    /// <summary>A validation error. </summary>
    public class ValidationError
    {
        /// <summary>Initializes a new instance of the <see cref="ValidationError"/> class. </summary>
        /// <param name="kind">The error kind. </param>
        /// <param name="property">The property name. </param>
        /// <param name="path">The property path. </param>
        public ValidationError(ValidationErrorKind kind, string property, string path)
        {
            Kind = kind; 
            Property = property;
            Path = path;
        }

        /// <summary>Gets the error kind. </summary>
        public ValidationErrorKind Kind { get; private set; }

        /// <summary>Gets the property name. </summary>
        public string Property { get; private set; }

        /// <summary>Gets the property path. </summary>
        public string Path { get; private set; }
    }
}
