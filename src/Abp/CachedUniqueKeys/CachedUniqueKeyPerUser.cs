using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;

namespace Abp.CachedUniqueKeys
{
    public class CachedUniqueKeyPerUser : ICachedUniqueKeyPerUser, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        private readonly ICacheManager _cacheManager;

        public CachedUniqueKeyPerUser(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            AbpSession = NullAbpSession.Instance;
        }

        public virtual async Task<string> GetKeyAsync(string cacheName)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            var cache = GetCache(cacheName);
            return await cache.GetAsync(GetKeyString(),
                () => Task.FromResult(Guid.NewGuid().ToString("N")));
        }

        public virtual async Task RemoveKeyAsync(string cacheName)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return;
            }

            var cache = GetCache(cacheName);
            await cache.RemoveAsync(GetKeyString());
        }

        public virtual async Task ClearCacheAsync(string cacheName)
        {
            var cache = GetCache(cacheName);
            await cache.ClearAsync();
        }

        public string GetKey(string cacheName)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            var cache = GetCache(cacheName);
            return cache.Get(GetKeyString(),
                () => Guid.NewGuid().ToString("N"));
        }

        public void RemoveKey(string cacheName)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return;
            }

            var cache = GetCache(cacheName);
            cache.Remove(GetKeyString());
        }

        public void ClearCache(string cacheName)
        {
            var cache = GetCache(cacheName);
            cache.Clear();
        }

        protected virtual ITypedCache<string, string> GetCache(string cacheName)
        {
            return _cacheManager.GetCache<string, string>(cacheName);
        }

        private string GetKeyString() => AbpSession.ToUserIdentifier().ToString();
    }
}