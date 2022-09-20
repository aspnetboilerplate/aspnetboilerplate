using System.Linq;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;
using Castle.Core.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Abp.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> to work with MemoryCache.
    /// </summary>
    public class AbpMemoryCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        public ILogger Logger { get; set; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public AbpMemoryCacheManager(ICachingConfiguration configuration)
            : base(configuration)
        {
            Logger = NullLogger.Instance;
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return new AbpMemoryCache(name, Configuration?.AbpConfiguration?.Caching?.MemoryCacheOptions)
            {
                Logger = Logger
            };
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches.Values)
            {
                cache.Dispose();
            }
        }
    }
}
