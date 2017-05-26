#if NET46
using System;
using System.Runtime.Caching;
using System.Collections.Generic;

namespace Abp.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICache"/> to work with <see cref="MemoryCache"/>.
    /// Implements <see cref="ICacheSupportsGetAllKeys"/> to work with <see cref="MemoryCache"/>.
    /// </summary>
    public class AbpMemoryCache : CacheBase, ICacheSupportsGetAllKeys
    {
        private MemoryCache _memoryCache;        
        private List<String> CacheKeys {get;set;}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Unique name of the cache</param>
        public AbpMemoryCache(string name)
            : base(name)
        {
            _memoryCache = new MemoryCache(Name);
            CacheKeys = new List<string>();
        }

        public override object GetOrDefault(string key)
        {
            return _memoryCache.Get(key);
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            var cachePolicy = new CacheItemPolicy();

            if (absoluteExpireTime != null)
            {
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(absoluteExpireTime.Value);
            }
            else if (slidingExpireTime != null)
            {
                cachePolicy.SlidingExpiration = slidingExpireTime.Value;
            }
            else if(DefaultAbsoluteExpireTime != null)
            {
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(DefaultAbsoluteExpireTime.Value);
            }
            else
            {
                cachePolicy.SlidingExpiration = DefaultSlidingExpireTime;
            }

            _memoryCache.Set(key, value, cachePolicy);
            if(CacheKeys==null)
                CacheKeys = new List<string>();
            if(!CacheKeys.Contains(key))
            {
                CacheKeys.Add(key);
            }
        }

        public override void Remove(string key)
        {
            _memoryCache.Remove(key);
            if(CacheKeys!=null)
                CacheKeys.Remove(key);
        }        

        public string[] GetAllKeys()
        {
            return CacheKeys.ToArray();
        }

        public override void Clear()
        {
            _memoryCache.Dispose();
            if(CacheKeys!=null)
                CacheKeys.Clear();
            _memoryCache = new MemoryCache(Name);
        }

        public override void Dispose()
        {
            _memoryCache.Dispose();
            base.Dispose();
        }
    }
}
#endif