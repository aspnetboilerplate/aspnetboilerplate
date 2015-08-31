using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheStore<in TKey, TValue>
    {
        string Name { get; }

        TimeSpan DefaultSlidingExpireTime { get; set; }

        TValue GetOrDefault(TKey key);

        Task<TValue> GetOrDefaultAsync(TKey key);

        void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null);

        Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null);

        void Remove(TKey key);
        
        Task RemoveAsync(TKey key);
    }
}