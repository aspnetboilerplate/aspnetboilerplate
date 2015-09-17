using System.Collections.Generic;
using Abp.Runtime.Caching;

namespace Abp.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="ICacheManager"/> to get setting caches.
    /// </summary>
    public static class CacheManagerSettingExtensions
    {
        /// <summary>
        /// Gets application settings cache.
        /// </summary>
        public static ITypedCache<string, Dictionary<string, SettingInfo>> GetApplicationSettingsCache(this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<string, Dictionary<string, SettingInfo>>(SettingManager.ApplicationSettingsCacheName);
        }

        /// <summary>
        /// Gets tenant settings cache.
        /// </summary>
        public static ITypedCache<int, Dictionary<string, SettingInfo>> GetTenantSettingsCache(this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<int, Dictionary<string, SettingInfo>>(SettingManager.TenantSettingsCacheName);
        }

        /// <summary>
        /// Gets user settings cache.
        /// </summary>
        public static ITypedCache<long, Dictionary<string, SettingInfo>> GetUserSettingsCache(this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<long, Dictionary<string, SettingInfo>>(SettingManager.UsersSettingsCacheName);
        }
    }
}
