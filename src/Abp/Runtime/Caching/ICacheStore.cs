using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheStore<in TKey, TValue> : ICacheStoreCommon
    {
        TValue GetOrDefault(TKey key);

        Task<TValue> GetOrDefaultAsync(TKey key);

        TValue GetOrCreate(TKey key, Func<TValue> factory);

        Task<TValue> GetOrCreateAsync(TKey key, Func<Task<TValue>> factory);

        void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null);

        Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null);

        void Remove(TKey key);
        
        Task RemoveAsync(TKey key);
    }
}