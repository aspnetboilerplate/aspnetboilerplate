using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.Extensions.Options;

namespace Abp.Runtime.Caching.Redis
{
    public class AbpRedisCacheKeyNormalizer : IAbpRedisCacheKeyNormalizer, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }
        protected AbpRedisCacheOptions RedisCacheOptions { get; }

        public AbpRedisCacheKeyNormalizer(
        IOptions<AbpRedisCacheOptions> redisCacheOptions)
        {
            AbpSession = NullAbpSession.Instance;
            RedisCacheOptions = redisCacheOptions.Value;
        }

        public string NormalizeKey(AbpRedisCacheKeyNormalizeArgs args)
        {
            var normalizedKey = $"n:{args.CacheName},c:{RedisCacheOptions.KeyPrefix}{args.Key}";

            if (args.MultiTenancyEnabled && AbpSession.TenantId != null && RedisCacheOptions.TenantKeyEnabled)
            {
                normalizedKey = $"t:{AbpSession.TenantId},{normalizedKey}";
            }

            return normalizedKey;
        }
    }
}
