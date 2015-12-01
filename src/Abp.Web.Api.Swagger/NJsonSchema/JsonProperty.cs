//-----------------------------------------------------------------------
// <copyright file="JsonProperty.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NJsonSchema
{
    /// <summary>A description of a JSON property of a JSON object. </summary>
    public class JsonProperty : JsonSchema4
    {
        private JsonSchema4 _parentSchema;

        internal static JsonProperty FromJsonSchema(string name, JsonSchema4 type)
        {
            var data = JsonConvert.SerializeObject(type);
            var property = JsonConvert.DeserializeObject<JsonProperty>(data);
            property.Name = name;
            return property;
        }
        
        /// <summary>Gets or sets the name of the property. </summary>
        [JsonIgnore]
        public string Name { get; internal set; }

        /// <summary>Gets the parent schema of this property schema. </summary>
        public override JsonSchema4 ParentSchema
        {
            get { return _parentSchema; }
            internal set
            {
                var initialize = _parentSchema == null;
                _parentSchema = value;

                if (initialize && InitialIsRequired)
                    IsRequired = InitialIsRequired;
            }
        }

        /// <summary>Gets or sets a value indicating whether the property is required. </summary>
        [JsonIgnore]
        public bool IsRequired
        {
            get { return ParentSchema.RequiredProperties.Contains(Name); }
            set
            {
                if (ParentSchema == null)
                    InitialIsRequired = value;
                else
                {
                    if (value)
                    {
                        if (!ParentSchema.RequiredProperties.Contains(Name))
                            ParentSchema.RequiredProperties.Add(Name);
                    }
                    else
                    {
                        if (ParentSchema.RequiredProperties.Contains(Name))
                            ParentSchema.RequiredProperties.Remove(Name);
                    }
                }
            }
        }

        /// <remarks>Value used to set <see cref="IsRequired"/> property even if parent is not set yet. </remarks>
        [JsonIgnore]
        internal bool InitialIsRequired { get; set; }
    }
}