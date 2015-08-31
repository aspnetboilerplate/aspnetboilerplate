using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Runtime.Caching
{
    public abstract class CacheProviderBase : ICacheProvider, IDisposable, ISingletonDependency
    {
        protected readonly IIocManager IocManager;

        protected readonly ConcurrentDictionary<string, object> CacheStores;

        protected CacheProviderBase(IIocManager iocManager)
        {
            IocManager = iocManager;
            CacheStores = new ConcurrentDictionary<string, object>();
        }

        public virtual ICacheStore<TKey, TValue> GetCacheStore<TKey, TValue>(string name)
        {
            var cacheStore = CacheStores.GetOrAdd(name, CreateCacheStore<TKey, TValue>);

            if (!(cacheStore is ICacheStore<TKey, TValue>))
            {
                throw new AbpException("Invalid cache store type request! Existing cache store's type is " + cacheStore.GetType().AssemblyQualifiedName + " but requested type is " + typeof(ICacheStore<TKey, TValue>).AssemblyQualifiedName);
            }

            return cacheStore as ICacheStore<TKey, TValue>;
        }

        public virtual Task<ICacheStore<TKey, TValue>> GetCacheStoreAsync<TKey, TValue>(string name)
        {
            return Task.FromResult(GetCacheStore<TKey, TValue>(name));
        }

        public void Dispose()
        {
            foreach (var cacheStore in CacheStores)
            {
                IocManager.Release(cacheStore);
            }
        }

        protected abstract ICacheStore<TKey, TValue> CreateCacheStore<TKey, TValue>(string name);
    }
}