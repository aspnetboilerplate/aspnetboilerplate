using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
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
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        public AbpRolePermissionCacheItemInvalidator(ICacheManager cacheManager, IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository)
        {
            _cacheManager = cacheManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
        }

        public void HandleEvent(EntityChangedEventData<RolePermissionSetting> eventData)
        {
            var cacheKey = eventData.Entity.RoleId + "@" + (eventData.Entity.TenantId ?? 0);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            var cacheKey = eventData.Entity.RoleId + "@" + (eventData.Entity.TenantId ?? 0);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);

            //get all users in organization unit
            var users = _userOrganizationUnitRepository.GetAllList(x =>
                x.OrganizationUnitId == eventData.Entity.OrganizationUnitId && x.TenantId == eventData.Entity.TenantId);
            //delete all users permission cache
            foreach (var userOrganizationUnit in users)
            {
                cacheKey = userOrganizationUnit.UserId + "@" + (eventData.Entity.TenantId ?? 0);
                _cacheManager.GetUserPermissionCache().Remove(cacheKey);
            }
        }

        public void HandleEvent(EntityDeletedEventData<AbpRoleBase> eventData)
        {
            var cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.TenantId ?? 0);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }
    }
}