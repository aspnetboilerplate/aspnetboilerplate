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

        public virtual ICacheStore<TKey, TValue> GetCacheStore<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime = null)
        {
            var cacheStore = CacheStores.GetOrAdd(name, n => CreateCacheStore<TKey, TValue>(n, defaultSlidingExpireTime));

            if (!(cacheStore is ICacheStore<TKey, TValue>))
            {
                throw new AbpException("Invalid cache store type request! Existing cache store's type is " + cacheStore.GetType().AssemblyQualifiedName + " but requested type is " + typeof(ICacheStore<TKey, TValue>).AssemblyQualifiedName);
            }

            return cacheStore as ICacheStore<TKey, TValue>;
        }

        public virtual Task<ICacheStore<TKey, TValue>> GetCacheStoreAsync<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime = null)
        {
            return Task.FromResult(GetCacheStore<TKey, TValue>(name, defaultSlidingExpireTime));
        }

        public virtual bool RemoveCacheStore(string name)
        {
            object cacheStoreObj;
            if (!CacheStores.TryRemove(name, out cacheStoreObj))
            {
                return false;
            }

            IocManager.Release(cacheStoreObj);
            return true;
        }

        public virtual Task<bool> RemoveCacheStoreAsync(string name)
        {
            return Task.FromResult(RemoveCacheStore(name));
        }

        public virtual void ClearAllCacheStores()
        {
            foreach (var cacheStore in CacheStores)
            {
                IocManager.Release(cacheStore);
            }

            CacheStores.Clear();
        }

        public virtual Task ClearAllCacheStoresAsync()
        {
            ClearAllCacheStores();
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {
            ClearAllCacheStores();
        }

        protected abstract ICacheStore<TKey, TValue> CreateCacheStore<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime);
    }
}