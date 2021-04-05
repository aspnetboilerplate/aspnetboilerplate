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

        public Task<string> GetKeyAsync(string cacheName, UserIdentifier user)
        {
            return GetKeyAsync(cacheName, user.TenantId, user.UserId);
        }

        public Task RemoveKeyAsync(string cacheName, UserIdentifier user)
        {
            return RemoveKeyAsync(cacheName, user.TenantId, user.UserId);
        }

        public virtual async Task<string> GetKeyAsync(string cacheName, int? tenantId, long? userId)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            var cache = GetCache(cacheName);
            return await cache.GetAsync(GetKeyString(tenantId, userId),
                () => Task.FromResult(Guid.NewGuid().ToString("N")));
        }

        public virtual async Task RemoveKeyAsync(string cacheName, int? tenantId, long? userId)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return;
            }

            var cache = GetCache(cacheName);
            await cache.RemoveAsync(GetKeyString(tenantId, userId));
        }

        public virtual async Task ClearCacheAsync(string cacheName)
        {
            var cache = GetCache(cacheName);
            await cache.ClearAsync();
        }

        public string GetKey(string cacheName, UserIdentifier user)
        {
            return GetKey(cacheName, user.TenantId, user.UserId);
        }

        public void RemoveKey(string cacheName, UserIdentifier user)
        {
            RemoveKey(cacheName, user.TenantId, user.UserId);
        }

        public string GetKey(string cacheName, int? tenantId, long? userId)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            var cache = GetCache(cacheName);
            return cache.Get(GetKeyString(tenantId, userId),
                () => Guid.NewGuid().ToString("N"));
        }

        public void RemoveKey(string cacheName, int? tenantId, long? userId)
        {
            if (!AbpSession.UserId.HasValue)
            {
                return;
            }

            var cache = GetCache(cacheName);
            cache.Remove(GetKeyString(tenantId, userId));
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

        private string GetKeyString(int? tenantId, long? userId)
        {
            if (tenantId == null)
            {
                return userId.ToString();
            }

            return userId + "@" + tenantId;
        }
    }
}