//-----------------------------------------------------------------------
// <copyright file="JsonSchema4.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema.Collections;
using NJsonSchema.Validation;

namespace NJsonSchema
{
    /// <summary>A base class for describing a JSON schema. </summary>
    public partial class JsonSchema4
    {
        private IDictionary<string, JsonProperty> _properties;
        private IDictionary<string, JsonProperty> _patternProperties;
        private IDictionary<string, JsonSchema4> _definitions;

        private ICollection<JsonSchema4> _allOf;
        private ICollection<JsonSchema4> _anyOf;
        private ICollection<JsonSchema4> _oneOf;
        private JsonSchema4 _not;

        private JsonSchema4 _item;
        private ICollection<JsonSchema4> _items;

        private bool _allowAdditionalItems = true;
        private JsonSchema4 _additionalItemsSchema = null;

        private bool _allowAdditionalProperties = true;
        private JsonSchema4 _additionalPropertiesSchema = null;
        private JsonSchema4 _schemaReference;

        /// <summary>Initializes a new instance of the <see cref="JsonSchema4"/> class. </summary>
        public JsonSchema4()
        {
            Initialize();
        }

        /// <summary>Creates a <see cref="JsonSchema4"/> from a given type. </summary>
        /// <typeparam name="TType">The type to create the schema for. </typeparam>
        /// <returns>The <see cref="JsonSchema4"/>. </returns>
        public static JsonSchema4 FromType<TType>()
        {
            var generator = new JsonSchemaGenerator();
            return generator.Generate<JsonSchema4>(typeof(TType), new SchemaResolver());
        }

        /// <summary>Creates a <see cref="JsonSchema4"/> from a given type. </summary>
        /// <param name="type">The type to create the schema for. </param>
        /// <returns>The <see cref="JsonSchema4"/>. </returns>
        public static JsonSchema4 FromType(Type type)
        {
            var generator = new JsonSchemaGenerator();
            return generator.Generate<JsonSchema4>(type, new SchemaResolver());
        }

        /// <summary>Deserializes a JSON string to a <see cref="JsonSchema4"/>. </summary>
        /// <param name="data">The JSON string. </param>
        /// <returns></returns>
        public static JsonSchema4 FromJson(string data)
        {
            data = JsonSchemaReferenceUtilities.ConvertJsonReferences(data);
            var schema = JsonConvert.DeserializeObject<JsonSchema4>(data, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.Default,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });

            JsonSchemaReferenceUtilities.UpdateSchemaReferences(schema);
            return schema;
        }

        internal static JsonSchema4 FromJsonWithoutReferenceHandling(string data)
        {
            var schema = JsonConvert.DeserializeObject<JsonSchema4>(data, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.Default,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            return schema;
        }

        /// <summary>Creates the type reference.</summary>
        /// <param name="schema">The referenced schema.</param>
        /// <returns>The type reference.</returns>
        public static JsonSchema4 CreateTypeReference(JsonSchema4 schema)
        {
            return new JsonSchema4
            {
                Type = JsonObjectType.Object,
                TypeName = schema.TypeName,
                SchemaReference = schema
            };
        }

        /// <summary>Gets or sets the schema. </summary>
        [JsonProperty("$schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string SchemaVersion { get; set; }

        /// <summary>Gets or sets the id. </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Id { get; set; }

        /// <summary>Gets or sets the title. </summary>
        [JsonProperty("title", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Title { get; set; }

        /// <summary>Gets or sets the description. </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets the object type. </summary>
        [JsonIgnore]
        public JsonObjectType Type { get; set; }

        /// <summary>Gets or sets the type reference path ($ref). </summary>
        [JsonProperty("schemaReferencePath", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal string SchemaReferencePath { get; set; }

        /// <summary>Gets or sets the type reference.</summary>
        [JsonIgnore]
        public JsonSchema4 SchemaReference
        {
            get { return _schemaReference; }
            set
            {
                if (_schemaReference != value)
                {
                    _schemaReference = value;
                    SchemaReferencePath = null; 
                }
            }
        }

        /// <summary>Gets a value indicating whether this is a type reference.</summary>
        [JsonIgnore]
        public bool HasSchemaReference
        {
            get { return SchemaReference != null; }
        }

        /// <summary>Gets the actual schema, either this or the reference schema.</summary>
        /// <exception cref="InvalidOperationException" accessor="get">The schema reference path has not been resolved.</exception>
        [JsonIgnore]
        public virtual JsonSchema4 ActualSchema
        {
            get
            {
                if (SchemaReferencePath != null && SchemaReference == null)
                    throw new InvalidOperationException("The schema reference path '" + SchemaReferencePath + "' has not been resolved.");

                return HasSchemaReference ? SchemaReference.ActualSchema : this;
            }
        }



        /// <summary>Gets the parent schema of this schema. </summary>
        [JsonIgnore]
        public virtual JsonSchema4 ParentSchema { get; internal set; }

        /// <summary>Gets or sets the format string. </summary>
        [JsonProperty("format", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Format { get; set; } // TODO: This is missing in JSON Schema schema

        /// <summary>Gets or sets the default value. </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Default { get; set; }

        /// <summary>Gets or sets the required multiple of for the number value. </summary>
        [JsonProperty("multipleOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public double? MultipleOf { get; set; } // TODO: Whats MultipleOf?

        /// <summary>Gets or sets the maximum allowed value. </summary>
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public double? Maximum { get; set; }

        /// <summary>Gets or sets a value indicating whether the maximum value is excluded. </summary>
        [JsonProperty("exclusiveMaximum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsExclusiveMaximum { get; set; }

        /// <summary>Gets or sets the minimum allowed value. </summary>
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public double? Minimum { get; set; }

        /// <summary>Gets or sets a value indicating whether the minimum value is excluded. </summary>
        [JsonProperty("exclusiveMinimum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsExclusiveMinimum { get; set; }

        /// <summary>Gets or sets the maximum length of the value string. </summary>
        [JsonProperty("maxLength", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? MaxLength { get; set; }

        /// <summary>Gets or sets the minimum length of the value string. </summary>
        [JsonProperty("minLength", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? MinLength { get; set; }

        /// <summary>Gets or sets the validation pattern as regular expression. </summary>
        [JsonProperty("pattern", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Pattern { get; set; }

        /// <summary>Gets or sets the maximum length of the array. </summary>
        [JsonProperty("maxItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MaxItems { get; set; }

        /// <summary>Gets or sets the minimum length of the array. </summary>
        [JsonProperty("minItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MinItems { get; set; }

        /// <summary>Gets or sets a value indicating whether the items in the array must be unique. </summary>
        [JsonProperty("uniqueItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UniqueItems { get; set; }

        /// <summary>Gets or sets the maximal number of allowed properties in an object. </summary>
        [JsonProperty("maxProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MaxProperties { get; set; }

        /// <summary>Gets or sets the minimal number of allowed properties in an object. </summary>
        [JsonProperty("minProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MinProperties { get; set; }

        /// <summary>Gets the collection of required properties. </summary>
        [JsonIgnore]
        public ICollection<object> Enumeration { get; internal set; }

        /// <summary>Gets the collection of required properties. </summary>
        /// <remarks>This collection can also be changed through the <see cref="JsonProperty.IsRequired"/> property. </remarks>>
        [JsonIgnore]
        public ICollection<string> RequiredProperties { get; internal set; }

        #region Child JSON schemas

        /// <summary>Gets the properties of the type. </summary>
        [JsonIgnore]
        public IDictionary<string, JsonProperty> Properties
        {
            get { return _properties; }
            internal set
            {
                if (_properties != value)
                {
                    RegisterProperties(_properties, value);
                    _properties = value;
                }
            }
        }

        /// <summary>Gets the pattern properties of the type. </summary>
        [JsonIgnore]
        public IDictionary<string, JsonProperty> PatternProperties
        {
            get { return _patternProperties; }
            internal set
            {
                if (_patternProperties != value)
                {
                    RegisterProperties(_patternProperties, value);
                    _patternProperties = value;
                }
            }
        }

        /// <summary>Gets or sets the schema of an array item. </summary>
        [JsonIgnore]
        public JsonSchema4 Item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    if (_item != null)
                    {
                        _item.ParentSchema = this;
                        Items.Clear();
                    }
                }
            }
        }

        /// <summary>Gets or sets the schema of an array item. </summary>
        [JsonIgnore]
        public ICollection<JsonSchema4> Items
        {
            get { return _items; }
            internal set
            {
                if (_items != value)
                {
                    RegisterSchemaCollection(_items, value);
                    _items = value;

                    if (_items != null)
                        Item = null;
                }
            }
        }

        /// <summary>Gets or sets the schema which must not be valid. </summary>
        [JsonProperty("not", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JsonSchema4 Not
        {
            get { return _not; }
            set
            {
                _not = value;
                if (_not != null)
                    _not.ParentSchema = this;
            }
        }

        /// <summary>Gets the other schema definitions of this schema. </summary>
        [JsonIgnore]
        public IDictionary<string, JsonSchema4> Definitions
        {
            get { return _definitions; }
            internal set
            {
                if (_definitions != value)
                {
                    RegisterSchemaDictionary(_definitions, value);
                    _definitions = value;
                }
            }
        }

        /// <summary>Gets the collection of schemas where each schema must be valid. </summary>
        [JsonIgnore]
        public ICollection<JsonSchema4> AllOf
        {
            get { return _allOf; }
            internal set
            {
                if (_allOf != value)
                {
                    RegisterSchemaCollection(_allOf, value);
                    _allOf = value;
                }
            }
        }

        /// <summary>Gets the collection of schemas where at least one must be valid. </summary>
        [JsonIgnore]
        public ICollection<JsonSchema4> AnyOf
        {
            get { return _anyOf; }
            internal set
            {
                if (_anyOf != value)
                {
                    RegisterSchemaCollection(_anyOf, value);
                    _anyOf = value;
                }
            }
        }

        /// <summary>Gets the collection of schemas where exactly one must be valid. </summary>
        [JsonIgnore]
        public ICollection<JsonSchema4> OneOf
        {
            get { return _oneOf; }
            internal set
            {
                if (_oneOf != value)
                {
                    RegisterSchemaCollection(_oneOf, value);
                    _oneOf = value;
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether additional items are allowed (default: true). </summary>
        /// <remarks>If this property is set to <c>false</c>, then <see cref="AdditionalItemsSchema"/> is set to <c>null</c>. </remarks>
        [JsonIgnore]
        public bool AllowAdditionalItems
        {
            get { return _allowAdditionalItems; }
            set
            {
                if (_allowAdditionalItems != value)
                {
                    _allowAdditionalItems = value;
                    if (!_allowAdditionalItems)
                        AdditionalItemsSchema = null;
                }
            }
        }

        /// <summary>Gets or sets the schema for the additional items. </summary>
        /// <remarks>If this property has a schema, then <see cref="AllowAdditionalItems"/> is set to <c>true</c>. </remarks>
        [JsonIgnore]
        public JsonSchema4 AdditionalItemsSchema
        {
            get { return _additionalItemsSchema; }
            set
            {
                if (_additionalItemsSchema != value)
                {
                    _additionalItemsSchema = value;
                    if (_additionalItemsSchema != null)
                        AllowAdditionalItems = true;
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether additional properties are allowed (default: true). </summary>
        /// <remarks>If this property is set to <c>false</c>, then <see cref="AdditionalPropertiesSchema"/> is set to <c>null</c>. </remarks>
        [JsonIgnore]
        public bool AllowAdditionalProperties
        {
            get { return _allowAdditionalProperties; }
            set
            {
                if (_allowAdditionalProperties != value)
                {
                    _allowAdditionalProperties = value;
                    if (!_allowAdditionalProperties)
                        AdditionalPropertiesSchema = null;
                }
            }
        }

        /// <summary>Gets or sets the schema for the additional properties. </summary>
        /// <remarks>If this property has a schema, then <see cref="AllowAdditionalProperties"/> is set to <c>true</c>. </remarks>
        [JsonIgnore]
        public JsonSchema4 AdditionalPropertiesSchema
        {
            get { return _additionalPropertiesSchema; }
            set
            {
                if (_additionalPropertiesSchema != value)
                {
                    _additionalPropertiesSchema = value;
                    if (_additionalPropertiesSchema != null)
                        AllowAdditionalProperties = true;
                }
            }
        }

        /// <summary>Gets a value indicating whether the schema represents a dictionary type (no properties and AdditionalProperties contains a schema).</summary>
        [JsonIgnore]
        public bool IsDictionary
        {
            get { return Properties.Count == 0 && AllowAdditionalProperties && AdditionalPropertiesSchema != null;}
        }

        #endregion

        /// <summary>Serializes the <see cref="JsonSchema4"/> to a JSON string. </summary>
        /// <returns>The JSON string. </returns>
        public string ToJson()
        {
            var oldSchema = SchemaVersion;
            SchemaVersion = "http://json-schema.org/draft-04/schema#";

            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(this);
            JsonSchemaReferenceUtilities.UpdateSchemaReferences(this);

            var data = JsonConvert.SerializeObject(this, Formatting.Indented);

            SchemaVersion = oldSchema;
            return JsonSchemaReferenceUtilities.ConvertPropertyReferences(data);
        }

        /// <summary>Validates the given JSON token against this schema. </summary>
        /// <param name="token">The token to validate. </param>
        /// <returns>The collection of validation errors. </returns>
        public ICollection<ValidationError> Validate(JToken token)
        {
            var validator = new JsonSchemaValidator(ActualSchema);
            return validator.Validate(token, null, null);
        }

        /// <summary>Finds the root parent of this schema. </summary>
        /// <returns>The parent schema or this when this is the root. </returns>
        public JsonSchema4 FindRootParent()
        {
            var parent = ParentSchema;
            if (parent == null)
                return this;

            while (parent.ParentSchema != null)
                parent = parent.ParentSchema;
            return parent;
        }

        private static JsonObjectType ConvertSimpleTypeFromString(string value)
        {
            // TODO: Improve performance
            return JsonConvert.DeserializeObject<JsonObjectType>("\"" + value + "\"");
        }

        private void Initialize()
        {
            if (Items == null)
                Items = new ObservableCollection<JsonSchema4>();

            if (Properties == null)
                Properties = new ObservableDictionary<string, JsonProperty>();

            if (PatternProperties == null)
                PatternProperties = new ObservableDictionary<string, JsonProperty>();

            if (Definitions == null)
                Definitions = new ObservableDictionary<string, JsonSchema4>();

            if (RequiredProperties == null)
                RequiredProperties = new ObservableCollection<string>();

            if (AllOf == null)
                AllOf = new ObservableCollection<JsonSchema4>();

            if (AnyOf == null)
                AnyOf = new ObservableCollection<JsonSchema4>();

            if (OneOf == null)
                OneOf = new ObservableCollection<JsonSchema4>();

            if (Enumeration == null)
                Enumeration = new Collection<object>();
        }
    }
}