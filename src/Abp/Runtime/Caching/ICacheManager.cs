using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// An upper level container for <see cref="ICache"/> objects. 
    /// A cache manager should work as Singleton and track and manage <see cref="ICache"/> objects.
    /// </summary>
    public interface ICacheManager : ICacheManager<ICache>
    {
    }

    public interface ICacheManager<TCache> : IDisposable
        where TCache : class
    {
        /// <summary>
        /// Gets all caches.
        /// </summary>
        /// <returns>List of caches</returns>
        IReadOnlyList<TCache> GetAllCaches();

        /// <summary>
        /// Gets a <see cref="ICache"/> instance.
        /// It may create the cache if it does not already exists.
        /// </summary>
        /// <param name="name">
        /// Unique and case sensitive name of the cache.
        /// </param>
        /// <returns>The cache reference</returns>
        [NotNull] TCache GetCache([NotNull] string name);
    }
}
