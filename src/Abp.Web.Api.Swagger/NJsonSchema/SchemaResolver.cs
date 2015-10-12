//-----------------------------------------------------------------------
// <copyright file="SchemaResolver.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NJsonSchema
{
    /// <summary>Manager which resolves types to schemas.</summary>
    public class SchemaResolver : ISchemaResolver
    {
        private readonly Dictionary<Type, JsonSchema4> _mappings = new Dictionary<Type, JsonSchema4>();

        /// <summary>Determines whether the specified type has a schema.</summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> when the mapping exists.</returns>
        public bool HasSchema(Type type)
        {
            return _mappings.ContainsKey(type);
        }

        /// <summary>Gets the schema for a given type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The schema.</returns>
        public JsonSchema4 GetSchema(Type type)
        {
            return _mappings[type];
        }

        /// <summary>Adds a schema to type mapping.</summary>
        /// <param name="type">The type.</param>
        /// <param name="schema">The schema.</param>
        public void AddSchema(Type type, JsonSchema4 schema)
        {
            _mappings.Add(type, schema);
        }

        /// <summary>Gets all the schemas.</summary>
        public IEnumerable<JsonSchema4> Schemes
        {
            get { return _mappings.Values; }
        }
    }
}