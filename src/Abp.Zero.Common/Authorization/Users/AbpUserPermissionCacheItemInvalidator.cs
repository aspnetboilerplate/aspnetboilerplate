using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Organizations;
using Abp.Runtime.Caching;

namespace Abp.Authorization.Users
{
    public class AbpUserPermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<UserPermissionSetting>>,
        IEventHandler<EntityChangedEventData<UserRole>>,
        IEventHandler<EntityChangedEventData<UserOrganizationUnit>>,
        IEventHandler<EntityDeletedEventData<AbpUserBase>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        public AbpUserPermissionCacheItemInvalidator(ICacheManager cacheManager, IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository)
        {
            _cacheManager = cacheManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
        }

        public void HandleEvent(EntityChangedEventData<UserPermissionSetting> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<UserOrganizationUnit> eventData)
        {
            var cacheKey = eventData.Entity.UserId + "@" + (eventData.Entity.TenantId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<AbpUserBase> eventData)
        {
            var cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.TenantId ?? 0);
            _cacheManager.GetUserPermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            //get all users in organization unit
            var users = _userOrganizationUnitRepository.GetAllList(x =>
                x.OrganizationUnitId == eventData.Entity.OrganizationUnitId && x.TenantId == eventData.Entity.TenantId);

            //delete all users permission cache
            foreach (var userOrganizationUnit in users)
            {
                var cacheKey = userOrganizationUnit.UserId + "@" + (eventData.Entity.TenantId ?? 0);
                _cacheManager.GetUserPermissionCache().Remove(cacheKey);
            }
        }
    }
}