using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;

namespace Abp.Caching
{
    public class GetScriptsResponsePerUserCache : ApplicationService, IGetScriptsResponsePerUserCache
    {
        private const string GetScriptsResponsePerUserCacheKey = "GetScriptsResponsePerUserCache";
        private readonly ITypedCache<string, string> _cache;

        public GetScriptsResponsePerUserCache(ICacheManager cacheManager)
        {
            _cache = cacheManager.GetCache<string, string>(GetScriptsResponsePerUserCacheKey);
        }

        public async Task<string> GetKey()
        {
            if (!AbpSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            return await _cache.GetAsync(GetKeyString(),
                () => Task.FromResult(Guid.NewGuid().ToString("N")));
        }

        public async Task RemoveKey()
        {
            if (!AbpSession.UserId.HasValue)
            {
                return;
            }

            await _cache.RemoveAsync(GetKeyString());
        }

        public async Task ClearAll()
        {
            await _cache.ClearAsync();
        }

        private string GetKeyString() => AbpSession.ToUserIdentifier().ToString();
    }
}