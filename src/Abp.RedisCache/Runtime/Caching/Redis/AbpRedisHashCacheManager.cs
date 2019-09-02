using System.Collections.Immutable;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to create <see cref="AbpRedisCache"/> instances.
    /// </summary>
    public class AbpRedisHashCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisCacheManager"/> class.
        /// </summary>
        public AbpRedisHashCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<AbpRedisHashCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<AbpRedisCache>(new { name });
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches)
            {
                _iocManager.Release(cache.Value);
            }
        }

        public IImmutableList<string> GetAllKeys(string cacheName)
        {
            var cache = GetCache(cacheName);

            if (!(cache is AbpRedisHashCache))
            {
                throw new AbpException("GetAllKeys method must be called on an cache of type " + nameof(AbpRedisHashCache));
            }

            return (cache as AbpRedisHashCache).GetAllKeys();
        }

        public IImmutableList<object> GetAllValues(string cacheName)
        {
            var cache = GetCache(cacheName);

            if (!(cache is AbpRedisHashCache))
            {
                throw new AbpException("GetAllKeys method must be called on an cache of type " + nameof(AbpRedisHashCache));
            }

            return (cache as AbpRedisHashCache).GetAllValues();
        }

        public bool Contains(string cacheName, string key)
        {
            var cache = GetCache(cacheName);

            if (!(cache is AbpRedisHashCache))
            {
                throw new AbpException("GetAllKeys method must be called on an cache of type " + nameof(AbpRedisHashCache));
            }

            return (cache as AbpRedisHashCache).Contains(key);
        }
    }
}