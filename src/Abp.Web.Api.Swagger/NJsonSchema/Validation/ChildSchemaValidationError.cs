//-----------------------------------------------------------------------
// <copyright file="ChildSchemaValidationError.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace NJsonSchema.Validation
{
    /// <summary>A subschema validation error. </summary>
    public class ChildSchemaValidationError : ValidationError
    {
        /// <summary>Initializes a new instance of the <see cref="ValidationError"/> class. </summary>
        /// <param name="kind">The error kind. </param>
        /// <param name="property">The property name. </param>
        /// <param name="path">The property path. </param>
        /// <param name="errors">The error list. </param>
        public ChildSchemaValidationError(ValidationErrorKind kind, string property, string path, IReadOnlyDictionary<JsonSchema4, ICollection<ValidationError>> errors)
            : base(kind, property, path)
        {
            Errors = errors;
        }
        
        /// <summary>Gets the errors for each validated subschema. </summary>
        public IReadOnlyDictionary<JsonSchema4, ICollection<ValidationError>> Errors { get; private set; } 
    }
}