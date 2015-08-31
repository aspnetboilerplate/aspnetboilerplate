using System;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Runtime.Caching
{
    public abstract class CacheStoreBase<TKey, TValue> : ICacheStore<TKey, TValue>, ITransientDependency, IDisposable
    {
        public string Name { get; private set; }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        protected CacheStoreBase(string name)
        {
            Name = name;
            DefaultSlidingExpireTime = TimeSpan.FromHours(2);
        }

        public abstract TValue GetOrDefault(TKey key);

        public virtual Task<TValue> GetOrDefaultAsync(TKey key)
        {
            return Task.FromResult(GetOrDefault(key));
        }

        public abstract void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null);

        public virtual Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null)
        {
            Set(key, value, slidingExpireTime);
            return Task.FromResult(0);
        }

        public abstract void Remove(TKey key);

        public virtual Task RemoveAsync(TKey key)
        {
            Remove(key);
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {

        }
    }
}