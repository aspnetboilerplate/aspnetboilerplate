using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Runtime.Caching.Redis.InMemory
{
    public abstract class AbpRedisInMemoryCacheBase : CacheBase, IAbpRedisInMemoryCache
    {
        public IIocManager IocManager { get; set; }

        protected AbpRedisInMemoryCacheBase(string name) : base(name)
        {

        }

        public abstract void SetInMemory(string key, object value);
        

        public void SetInMemory(KeyValuePair<string, object>[] pairs)
        {
            foreach (var pair in pairs)
            {
                SetInMemory(pair.Key, pair.Value);
            }
        }

        public Task SetInMemoryAsync(string key, object value)
        {
            Set(key, value);
            return Task.FromResult(0);
        }

        public Task SetInMemoryAsync(KeyValuePair<string, object>[] pairs)
        {
            return Task.WhenAll(pairs.Select(p => SetInMemoryAsync(p.Key, p.Value)));
        }

        public abstract void ClearMemory();

        public virtual Task ClearMemoryAsync()
        {
            Clear();
            return Task.FromResult(0);
        }
    }
}
