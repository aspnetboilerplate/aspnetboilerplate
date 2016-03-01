using Adorable.Dependency;
using Adorable.Runtime.Caching;
using Adorable.Runtime.Caching.Configuration;

namespace Adorable.RedisCache
{
    public class AbpRedisCacheManager : CacheManagerBase
    {
        public AbpRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(iocManager, configuration)
        {
            IocManager.RegisterIfNot<AbpRedisCache>(DependencyLifeStyle.Transient);
        }
        protected override ICache CreateCacheImplementation(string name)
        {
            return IocManager.Resolve<AbpRedisCache>(new { name });
        }
    }
}
