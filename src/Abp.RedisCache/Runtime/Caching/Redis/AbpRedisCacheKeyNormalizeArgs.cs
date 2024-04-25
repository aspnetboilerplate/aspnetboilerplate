namespace Abp.Runtime.Caching.Redis
{
    public class AbpRedisCacheKeyNormalizeArgs
    {
        public string Key { get; }

        public string CacheName { get; }

        public bool MultiTenancyEnabled { get; }

        public AbpRedisCacheKeyNormalizeArgs(
            string key,
            string cacheName,
            bool multiTenancyEnabled)
        {
            Key = key;
            CacheName = cacheName;
            MultiTenancyEnabled = multiTenancyEnabled;
        }
    }
}
