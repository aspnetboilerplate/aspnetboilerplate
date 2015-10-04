using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;

namespace Abp.RedisCache
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
