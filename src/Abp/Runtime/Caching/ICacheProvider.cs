using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheProvider
    {
        ICacheStore<TKey, TValue> GetCacheStore<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime = null);

        Task<ICacheStore<TKey, TValue>> GetCacheStoreAsync<TKey, TValue>(string name, TimeSpan? defaultSlidingExpireTime = null);

        bool RemoveCacheStore(string name);

        Task<bool> RemoveCacheStoreAsync(string name);

        void ClearAllCacheStores();

        Task ClearAllCacheStoresAsync();
    }
}
