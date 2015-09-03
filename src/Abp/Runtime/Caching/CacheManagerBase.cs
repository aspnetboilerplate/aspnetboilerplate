using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Base class for cache managers.
    /// </summary>
    public abstract class CacheManagerBase : ICacheManager, ISingletonDependency
    {
        protected readonly IIocManager IocManager;

        protected readonly ConcurrentDictionary<string, ICache> Caches;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="iocManager"></param>
        protected CacheManagerBase(IIocManager iocManager)
        {
            IocManager = iocManager;
            Caches = new ConcurrentDictionary<string, ICache>();
        }

        public IReadOnlyList<ICache> GetAllCaches()
        {
            return Caches.Values.ToImmutableList();
        }
        
        public virtual ICache GetCache(string name)
        {
            //TODO: Get expire time from some configuration!
            return Caches.GetOrAdd(name, CreateCacheImplementation);
        }

        public virtual void Dispose()
        {
            foreach (var cacheStore in Caches)
            {
                IocManager.Release(cacheStore.Value);
            }

            Caches.Clear();
        }

        /// <summary>
        /// Used to create actual cache implementation.
        /// </summary>
        /// <param name="name">Name of the cache</param>
        /// <returns>Cache object</returns>
        protected abstract ICache CreateCacheImplementation(string name);
    }
}