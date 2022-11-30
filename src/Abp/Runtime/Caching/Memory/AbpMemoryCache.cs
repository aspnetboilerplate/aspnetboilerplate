using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Abp.Data;

namespace Abp.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICache"/> to work with <see cref="MemoryCache"/>.
    /// </summary>
    public class AbpMemoryCache : CacheBase
    {
        private MemoryCache _memoryCache;
        private readonly MemoryCacheOptions _memoryCacheOptions;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Unique name of the cache</param>
        /// <param name="memoryCacheOptions">MemoryCacheOptions</param>
        public AbpMemoryCache(string name, MemoryCacheOptions memoryCacheOptions = null)
            : base(name)
        {
            _memoryCacheOptions = memoryCacheOptions;
            _memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(
                memoryCacheOptions ?? new MemoryCacheOptions()
            ));
        }

        public override bool TryGetValue(string key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            if (value == null)
            {
                throw new AbpException("Can not insert null values to the cache!");
            }

            if (absoluteExpireTime.HasValue || slidingExpireTime.HasValue)
            {
                var cacheOptions = new MemoryCacheEntryOptions();

                if (absoluteExpireTime.HasValue)
                {
                    cacheOptions.AbsoluteExpiration = absoluteExpireTime;
                }

                if (slidingExpireTime.HasValue)
                {
                    cacheOptions.SlidingExpiration = slidingExpireTime;
                }
                
                _memoryCache.Set(key, value, cacheOptions);
            }
            else if (DefaultAbsoluteExpireTimeFactory != null)
            {
                _memoryCache.Set(key, value, DefaultAbsoluteExpireTimeFactory(key));
            }
            else if (DefaultAbsoluteExpireTime.HasValue)
            {
                _memoryCache.Set(key, value, DefaultAbsoluteExpireTime.Value);
            }
            else
            {
                _memoryCache.Set(key, value, DefaultSlidingExpireTime);
            }
        }

        public override void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public override void Clear()
        {
            _memoryCache.Dispose();
            _memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(
                _memoryCacheOptions ?? new MemoryCacheOptions()
            ));
        }

        public override void Dispose()
        {
            _memoryCache.Dispose();
            base.Dispose();
        }
    }
}
