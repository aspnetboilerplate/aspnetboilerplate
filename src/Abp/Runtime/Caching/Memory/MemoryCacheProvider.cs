using System;
using Abp.Dependency;

namespace Abp.Runtime.Caching.Memory
{
    public class MemoryCacheProvider : CacheProviderBase
    {
        public MemoryCacheProvider(IIocManager iocManager)
            : base(iocManager)
        {
        }

        protected override ICacheStore<TKey, TValue> CreateCacheStore<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime)
        {
            IocManager.RegisterIfNot<MemoryCacheStore<TKey, TValue>>(DependencyLifeStyle.Transient);
            return IocManager.Resolve<MemoryCacheStore<TKey, TValue>>(new { name, defaultSlidingExpireTime });
        }
    }
}
