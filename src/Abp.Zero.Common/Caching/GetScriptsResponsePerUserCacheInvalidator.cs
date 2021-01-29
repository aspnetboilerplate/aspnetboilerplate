using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Localization;
using Abp.Organizations;

namespace Abp.Caching
{
    public class GetScriptsResponsePerUserCacheInvalidator :
        IEventHandler<EntityChangedEventData<UserPermissionSetting>>,
        IEventHandler<EntityChangedEventData<UserRole>>,
        IEventHandler<EntityChangedEventData<UserOrganizationUnit>>,
        IEventHandler<EntityDeletedEventData<AbpUserBase>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        IEventHandler<EntityChangedEventData<LanguageInfo>>,
        IEventHandler<EntityChangedEventData<SettingInfo>>,
        ITransientDependency
    {
        private readonly IGetScriptsResponsePerUserCache _getScriptsResponsePerUserCache;

        public GetScriptsResponsePerUserCacheInvalidator(IGetScriptsResponsePerUserCache getScriptsResponsePerUserCache)
        {
            _getScriptsResponsePerUserCache = getScriptsResponsePerUserCache;
        }

        public void HandleEvent(EntityChangedEventData<UserPermissionSetting> eventData)
        {
            RemoveCache();
        }

        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            RemoveCache();
        }

        public void HandleEvent(EntityChangedEventData<UserOrganizationUnit> eventData)
        {
            RemoveCache();
        }

        public void HandleEvent(EntityDeletedEventData<AbpUserBase> eventData)
        {
            RemoveCache();
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            RemoveCache();
        }

        public void HandleEvent(EntityChangedEventData<LanguageInfo> eventData)
        {
            RemoveCache();
        }

        public void HandleEvent(EntityChangedEventData<SettingInfo> eventData)
        {
            _getScriptsResponsePerUserCache.ClearAll();
        }

        private void RemoveCache()
        {
            _getScriptsResponsePerUserCache.RemoveKey();
        }
    }
}