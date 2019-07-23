using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching
{
    [Obsolete("Use CacheManagerBase<TCache> instead.")]
    public abstract class CacheManagerBase : CacheManagerBase<ICache>, ICacheManager
    {
        public CacheManagerBase(ICachingConfiguration configuration) : base(configuration)
        {
        }
    }

    /// <summary>
    /// Base class for cache managers.
    /// </summary>
    public abstract class CacheManagerBase<TCache> : ICacheManager<TCache>, ISingletonDependency
        where TCache : class, ICacheOptions
    {

        protected readonly ICachingConfiguration Configuration;

        protected readonly ConcurrentDictionary<string, TCache> Caches;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration"></param>
        protected CacheManagerBase(ICachingConfiguration configuration)
        {
            Configuration = configuration;
            Caches = new ConcurrentDictionary<string, TCache>();
        }

        public IReadOnlyList<TCache> GetAllCaches()
        {
            return Caches.Values.ToImmutableList();
        }

        public virtual TCache GetCache(string name)
        {
            Check.NotNull(name, nameof(name));

            return Caches.GetOrAdd(name, (cacheName) =>
            {
                var cache = CreateCacheImplementation(cacheName);

                var configurators = Configuration.Configurators.Where(c => c.CacheName == null || c.CacheName == cacheName);

                foreach (var configurator in configurators)
                {
                    configurator.InitAction?.Invoke(cache);
                }

                return cache;
            });
        }
        protected abstract void DisposeCaches();
        public virtual void Dispose()
        {
            DisposeCaches();
            Caches.Clear();
        }

        /// <summary>
        /// Used to create actual cache implementation.
        /// </summary>
        /// <param name="name">Name of the cache</param>
        /// <returns>Cache object</returns>
        protected abstract TCache CreateCacheImplementation(string name);
    }
}
