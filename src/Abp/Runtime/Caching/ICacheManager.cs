using System;
using System.Collections.Generic;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// An upper level container for <see cref="ICache"/> objects. 
    /// A cache manager should work as Singleton and track and manage <see cref="ICache"/> objects.
    /// </summary>
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        /// Gets all caches.
        /// </summary>
        /// <returns>List of caches</returns>
        IReadOnlyList<ICache> GetAllCaches();

        /// <summary>
        /// Gets (or creates) a cache.
        /// </summary>
        /// <param name="name">Unique name of the cache</param>
        /// <returns>The cache reference</returns>
        ICache GetCache(string name);
    }
}
