//-----------------------------------------------------------------------
// <copyright file="JsonObjectType.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace NJsonSchema
{
    /// <summary>Enumeration of the possible object types. </summary>
    [Flags]
    public enum JsonObjectType
    {
        /// <summary>No object type. </summary>
        [JsonProperty("none")]
        None = 0,

        /// <summary>An array. </summary>
        [JsonProperty("array")]
        Array = 1,

        /// <summary>A boolean value. </summary>
        [JsonProperty("boolean")]
        Boolean = 2,

        /// <summary>An integer value. </summary>
        [JsonProperty("integer")]
        Integer = 4,

        /// <summary>A null. </summary>
        [JsonProperty("null")]
        Null = 8, 

        /// <summary>An number value. </summary>
        [JsonProperty("number")]
        Number = 16,

        /// <summary>An object. </summary>
        [JsonProperty("object")]
        Object = 32,

        /// <summary>A string. </summary>
        [JsonProperty("string")]
        String = 64, 
    }
}