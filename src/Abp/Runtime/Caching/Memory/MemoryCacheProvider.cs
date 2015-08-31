using Abp.Dependency;

namespace Abp.Runtime.Caching.Memory
{
    public class MemoryCacheProvider : CacheProviderBase
    {
        public MemoryCacheProvider(IIocManager iocManager)
            : base(iocManager)
        {
        }

        protected override ICacheStore<TKey, TValue> CreateCacheStore<TKey, TValue>(string name)
        {
            IocManager.RegisterIfNot<MemoryCacheStore<TKey, TValue>>();
            return IocManager.Resolve<MemoryCacheStore<TKey, TValue>>(new {name});
        }
    }
}
