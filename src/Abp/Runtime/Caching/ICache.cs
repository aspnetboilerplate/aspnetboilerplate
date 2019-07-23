using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Defines a cache that can be store and get items by keys.
    /// </summary>
    public interface ICache : IDisposable, ICacheOptions, ICacheOperations, ICacheSingleKeyOperations<string, object>, ICacheMultipleKeysOperations<string, object>
    {
        /// <summary>
        /// Unique name of the cache.
        /// </summary>
        string Name { get; }
    }
}
