using Abp.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Abp.Reflection;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// A class to hold the Type information and Serialized payload for data stored in the cache.
    /// </summary>
    public class AbpCacheData
    {
        public AbpCacheData(
            string type, string payload)
        {
            Type = type;
            Payload = payload;
        }

        public string Payload { get; set; }

        public string Type { get; set; }

        public static AbpCacheData Deserialize(string serializedCacheData) => serializedCacheData.FromJsonString<AbpCacheData>();

        public static AbpCacheData Serialize(object obj, bool withAssemblyName = true)
        {
            return new AbpCacheData(
                TypeHelper.SerializeType(obj.GetType(), withAssemblyName).ToString(),
                obj.ToJsonString());
        }
        
    }
}