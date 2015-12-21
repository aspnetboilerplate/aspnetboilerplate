//-----------------------------------------------------------------------
// <copyright file="JsonSchemaGenerator.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace NJsonSchema
{
    /// <summary>Generates a <see cref="JsonSchema4"/> object for a given type. </summary>
    public class JsonSchemaGenerator
    {
        /// <summary>Generates a <see cref="JsonSchema4" /> object for the given type and adds the mapping to the given resolver.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <returns>The schema.</returns>
        /// <exception cref="InvalidOperationException">Could not find value type of dictionary type.</exception>
        /// <exception cref="InvalidOperationException">Could not find item type of enumeration type.</exception>
        public TSchemaType Generate<TSchemaType>(Type type, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            var schema = new TSchemaType();

            var typeDescription = JsonObjectTypeDescription.FromType(type);
            schema.Type = typeDescription.Type;
            schema.Format = typeDescription.Format;

            if (schema.Type.HasFlag(JsonObjectType.Object))
            {
                if (typeDescription.IsDictionary)
                    GenerateDictionary(type, schema, schemaResolver);
                else
                {
                    schema.TypeName = type.Name;

                    if (schemaResolver.HasSchema(type))
                    {
                        schema.SchemaReference = schemaResolver.GetSchema(type);
                        return schema;
                    }

                    GenerateObject(type, schema, schemaResolver);
                }
            }
            else if (schema.Type.HasFlag(JsonObjectType.Array))
            {
                schema.Type = JsonObjectType.Array;

                var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
                if (itemType == null)
                    throw new InvalidOperationException("Could not find item type of enumeration type '" + type.FullName + "'.");

                schema.Item = Generate<JsonSchema4>(itemType, schemaResolver);
            }

            TryLoadEnumerations(type, schema);
            return schema;
        }

        /// <exception cref="InvalidOperationException">Could not find value type of dictionary type.</exception>
        private void GenerateDictionary<TSchemaType>(Type type, TSchemaType schema, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            if (type.GenericTypeArguments.Length != 2)
                throw new InvalidOperationException("Could not find value type of dictionary type '" + type.FullName + "'.");

            var valueType = type.GenericTypeArguments[1];

            schema.AdditionalPropertiesSchema = Generate<JsonProperty>(valueType, schemaResolver);
            schema.AllowAdditionalProperties = true;
        }

        /// <summary>Generates the properties for the given type and schema.</summary>
        /// <typeparam name="TSchemaType">The type of the schema type.</typeparam>
        /// <param name="type">The types.</param>
        /// <param name="schema">The properties</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        protected virtual void GenerateObject<TSchemaType>(Type type, TSchemaType schema, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            schemaResolver.AddSchema(type, schema);

            schema.TypeName = type.Name;
            schema.AllowAdditionalProperties = false;
            foreach (var property in type.GetRuntimeProperties())
                LoadProperty(property, schema, schemaResolver);
        }

        private static void TryLoadEnumerations<TSchemaType>(Type type, TSchemaType schema)
            where TSchemaType : JsonSchema4, new()
        {
            if (type.GetTypeInfo().IsEnum)
            {
                foreach (var enumValue in Enum.GetNames(type))
                    schema.Enumeration.Add(enumValue);
            }
        }

        private void LoadProperty<TSchemaType>(PropertyInfo property, TSchemaType parentSchema, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            var propertyType = property.PropertyType;
            var propertyTypeDescription = JsonObjectTypeDescription.FromType(propertyType);

            var attributes = property.GetCustomAttributes().ToArray();

            var jsonProperty = Generate<JsonProperty>(propertyType, schemaResolver);

            var propertyName = property.Name;
            var jsonPropertyAttribute = attributes.FirstOrDefault(a => a is JsonPropertyAttribute) as JsonPropertyAttribute;
            if (jsonPropertyAttribute != null && !string.IsNullOrEmpty(jsonPropertyAttribute.PropertyName))
                propertyName = jsonPropertyAttribute.PropertyName;

            parentSchema.Properties.Add(propertyName, jsonProperty);

            var requiredAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.RequiredAttribute");
            if (propertyTypeDescription.IsAlwaysRequired || requiredAttribute != null)
                parentSchema.RequiredProperties.Add(property.Name);

            dynamic descriptionAttribute = TryGetAttribute(attributes, "System.ComponentModel.DescriptionAttribute");
            if (descriptionAttribute != null)
                jsonProperty.Description = descriptionAttribute.Description;

            dynamic regexAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.RegularExpressionAttribute");
            if (regexAttribute != null)
                jsonProperty.Pattern = regexAttribute.Pattern;

            dynamic rangeAttribute = TryGetAttribute(attributes, "System.ComponentModel.DataAnnotations.RangeAttribute");
            if (rangeAttribute != null)
            {
                if (rangeAttribute.Minimum != null)
                    jsonProperty.Minimum = rangeAttribute.Minimum;
                if (rangeAttribute.Maximum != null)
                    jsonProperty.Maximum = rangeAttribute.Maximum;
            }
        }

        private Attribute TryGetAttribute(Attribute[] attributes, string attributeType)
        {
            return attributes.FirstOrDefault(a => a.GetType().FullName == attributeType);
        }
    }
}
