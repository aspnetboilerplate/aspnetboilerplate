using System.Linq;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Organizations;
using Abp.Runtime.Caching;

namespace Abp.Authorization.Roles
{
    public class AbpRolePermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<RolePermissionSetting>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        IEventHandler<EntityDeletedEventData<AbpRoleBase>>,
        ITransientDependency
    {
        private readonly ICacheManager _cacheManager;

        public AbpRolePermissionCacheItemInvalidator(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<RolePermissionSetting> eventData)
        {
            InvalidateRoleCache(eventData.Entity.RoleId, eventData.Entity.TenantId);
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            InvalidateRoleCache(eventData.Entity.RoleId, eventData.Entity.TenantId);
        }

        public void HandleEvent(EntityDeletedEventData<AbpRoleBase> eventData)
        {
            InvalidateRoleCache(eventData.Entity.Id, eventData.Entity.TenantId);
        }

        private void InvalidateRoleCache(int roleId, long? tenantId)
        {
            var cacheKey = roleId + "@" + (tenantId ?? 0);
            var cache = _cacheManager.GetRolePermissionCache();
            var effectedKeys = cache.Keys.Where(e => e.StartsWith(cacheKey)).ToList();
            foreach (var key in effectedKeys)
            {
                cache.Remove(key);
            }
        }
    }
}