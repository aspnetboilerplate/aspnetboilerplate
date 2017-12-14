using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;
using System;

namespace Abp.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> to work with MemoryCache.
    /// </summary>
    public class AbpMemoryCacheManager : CacheManagerBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpMemoryCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(iocManager, configuration)
        {
            IocManager.RegisterIfNot<AbpMemoryCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            Action<string> disposeAction = (cacheName) => Caches.TryRemove(cacheName, out ICache cache);
            return IocManager.Resolve<AbpMemoryCache>(new { name, disposeAction });
        }
    }
}
