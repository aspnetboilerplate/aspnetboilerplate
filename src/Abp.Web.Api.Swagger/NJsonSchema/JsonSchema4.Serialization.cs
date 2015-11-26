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
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema.Collections;

namespace NJsonSchema
{
    public partial class JsonSchema4
    {
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext ctx)
        {
            Initialize();
        }

        /// <summary>Gets or sets the type name (class name of the object). </summary>
        [JsonProperty("typeName", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string TypeName { get; set; }


        [JsonProperty("additionalItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal object AdditionalItemsRaw
        {
            get
            {
                if (AdditionalItemsSchema != null)
                    return AdditionalItemsSchema;
                if (!AllowAdditionalItems)
                    return false;
                return null;
            }
            set
            {
                if (value is bool)
                    AllowAdditionalItems = (bool)value;
                else if (value != null)
                    AdditionalItemsSchema = FromJsonWithoutReferenceHandling(value.ToString());
            }
        }

        [JsonProperty("additionalProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal object AdditionalPropertiesRaw
        {
            get
            {
                if (AdditionalPropertiesSchema != null)
                    return AdditionalPropertiesSchema;
                if (!AllowAdditionalProperties)
                    return false;
                return null;
            }
            set
            {
                if (value is bool)
                    AllowAdditionalProperties = (bool)value;
                else if (value != null)
                    AdditionalPropertiesSchema = FromJsonWithoutReferenceHandling(value.ToString());
            }
        }

        [JsonProperty("items", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal object ItemsRaw
        {
            get
            {
                if (Item != null)
                    return Item;
                if (Items.Count > 0)
                    return Items;
                return null;
            }
            set
            {
                if (value is JArray)
                    Items = new ObservableCollection<JsonSchema4>(((JArray)value).Select(t => FromJsonWithoutReferenceHandling(t.ToString())));
                else if (value != null)
                    Item = FromJsonWithoutReferenceHandling(value.ToString());
            }
        }

        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal object TypeRaw
        {
            get
            {
                var flags = Enum.GetValues(Type.GetType())
                    .Cast<Enum>().Where(v => Type.HasFlag(v))
                    .OfType<JsonObjectType>()
                    .Where(v => v != JsonObjectType.None)
                    .ToArray();

                if (flags.Length > 1)
                    return new JArray(flags.Select(f => new JValue(f.ToString().ToLower())));
                if (flags.Length == 1)
                    return new JValue(flags[0].ToString().ToLower());

                return null;
            }
            set
            {
                if (value is JArray)
                    Type = ((JArray)value).Aggregate(JsonObjectType.None, (type, token) => type | ConvertSimpleTypeFromString(token.ToString()));
                else if (value != null)
                    Type = ConvertSimpleTypeFromString(value.ToString());
                else
                    Type = JsonObjectType.None;
            }
        }

        [JsonProperty("required", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal ICollection<string> RequiredPropertiesRaw
        {
            get { return RequiredProperties != null && RequiredProperties.Count > 0 ? RequiredProperties : null; }
            set { RequiredProperties = value; }
        }

        [JsonProperty("properties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal IDictionary<string, JsonSchema4> PropertiesRaw
        {
            get
            {
                return Properties != null && Properties.Count > 0 ?
                    Properties.ToDictionary(p => p.Key, p => (JsonSchema4)p.Value) : null;
            }
            set
            {
                Properties = value != null ?
                    new ObservableDictionary<string, JsonProperty>(value.ToDictionary(p => p.Key, p => JsonProperty.FromJsonSchema(p.Key, p.Value))) :
                    new ObservableDictionary<string, JsonProperty>();
            }
        }

        [JsonProperty("patternProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal IDictionary<string, JsonSchema4> PatternPropertiesRaw
        {
            get
            {
                return PatternProperties != null && PatternProperties.Count > 0 ?
                    PatternProperties.ToDictionary(p => p.Key, p => (JsonSchema4)p.Value) : null;
            }
            set
            {
                PatternProperties = value != null ?
                    new ObservableDictionary<string, JsonProperty>(value.ToDictionary(p => p.Key, p => JsonProperty.FromJsonSchema(p.Key, p.Value))) :
                    new ObservableDictionary<string, JsonProperty>();
            }
        }

        [JsonProperty("definitions", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal IDictionary<string, JsonSchema4> DefinitionsRaw
        {
            get { return Definitions != null && Definitions.Count > 0 ? Definitions : null; }
            set { Definitions = value != null ? new ObservableDictionary<string, JsonSchema4>(value) : new ObservableDictionary<string, JsonSchema4>(); }
        }

        [JsonProperty("enum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal ICollection<object> EnumerationRaw
        {
            get { return Enumeration != null && Enumeration.Count > 0 ? Enumeration : null; }
            set { Enumeration = value != null ? new ObservableCollection<object>(value) : new ObservableCollection<object>(); }
        }

        [JsonProperty("allOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal ICollection<JsonSchema4> AllOfRaw
        {
            get { return AllOf != null && AllOf.Count > 0 ? AllOf : null; }
            set { AllOf = value != null ? new ObservableCollection<JsonSchema4>(value) : new ObservableCollection<JsonSchema4>(); }
        }

        [JsonProperty("anyOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal ICollection<JsonSchema4> AnyOfRaw
        {
            get { return AnyOf != null && AnyOf.Count > 0 ? AnyOf : null; }
            set { AnyOf = value != null ? new ObservableCollection<JsonSchema4>(value) : new ObservableCollection<JsonSchema4>(); }
        }

        [JsonProperty("oneOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        internal ICollection<JsonSchema4> OneOfRaw
        {
            get { return OneOf != null && OneOf.Count > 0 ? OneOf : null; }
            set { OneOf = value != null ? new ObservableCollection<JsonSchema4>(value) : new ObservableCollection<JsonSchema4>(); }
        }

        private void RegisterProperties(IDictionary<string, JsonProperty> oldCollection, IDictionary<string, JsonProperty> newCollection)
        {
            if (oldCollection != null)
                ((ObservableDictionary<string, JsonProperty>)oldCollection).CollectionChanged -= InitializeSchemaCollection;

            if (newCollection != null)
            {
                ((ObservableDictionary<string, JsonProperty>)newCollection).CollectionChanged += InitializeSchemaCollection;
                InitializeSchemaCollection(newCollection, null);
            }
        }

        private void RegisterSchemaDictionary(IDictionary<string, JsonSchema4> oldCollection, IDictionary<string, JsonSchema4> newCollection)
        {
            if (oldCollection != null)
                ((ObservableDictionary<string, JsonSchema4>)oldCollection).CollectionChanged -= InitializeSchemaCollection;

            if (newCollection != null)
            {
                ((ObservableDictionary<string, JsonSchema4>)newCollection).CollectionChanged += InitializeSchemaCollection;
                InitializeSchemaCollection(newCollection, null);
            }
        }

        private void RegisterSchemaCollection(ICollection<JsonSchema4> oldCollection, ICollection<JsonSchema4> newCollection)
        {
            if (oldCollection != null)
                ((ObservableCollection<JsonSchema4>)oldCollection).CollectionChanged -= InitializeSchemaCollection;

            if (newCollection != null)
            {
                ((ObservableCollection<JsonSchema4>)newCollection).CollectionChanged += InitializeSchemaCollection;
                InitializeSchemaCollection(newCollection, null);
            }
        }

        private void InitializeSchemaCollection(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableDictionary<string, JsonProperty>)
            {
                var properties = (ObservableDictionary<string, JsonProperty>)sender;
                foreach (var property in properties)
                {
                    property.Value.Name = property.Key;
                    property.Value.ParentSchema = this;
                }
            }
            else if (sender is ObservableCollection<JsonSchema4>)
            {
                var collection = (ObservableCollection<JsonSchema4>)sender;
                foreach (var item in collection)
                    item.ParentSchema = this;
            }
            else if (sender is ObservableDictionary<string, JsonSchema4>)
            {
                var collection = (ObservableDictionary<string, JsonSchema4>)sender;
                foreach (var item in collection.Values)
                    item.ParentSchema = this;
            }
        }
    }
}