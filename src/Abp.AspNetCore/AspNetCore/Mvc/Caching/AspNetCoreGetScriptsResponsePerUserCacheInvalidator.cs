using Abp.CachedUniqueKeys;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Localization;

namespace Abp.AspNetCore.Mvc.Caching
{
    public class AspNetCoreGetScriptsResponsePerUserCacheInvalidator :
        IEventHandler<EntityChangedEventData<LanguageInfo>>,
        IEventHandler<EntityChangedEventData<SettingInfo>>,
        ITransientDependency
    {
        private const string CacheName = "GetScriptsResponsePerUser";

        private readonly ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;

        public AspNetCoreGetScriptsResponsePerUserCacheInvalidator(ICachedUniqueKeyPerUser cachedUniqueKeyPerUser)
        {
            _cachedUniqueKeyPerUser = cachedUniqueKeyPerUser;
        }

        public void HandleEvent(EntityChangedEventData<LanguageInfo> eventData)
        {
            RemoveCache();
        }

        public void HandleEvent(EntityChangedEventData<SettingInfo> eventData)
        {
            _cachedUniqueKeyPerUser.ClearCache(CacheName);
        }

        private void RemoveCache()
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName);
        }
    }
}