using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
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
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AbpUserPermissionCacheItemInvalidator(
            ICacheManager cacheManager, 
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
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

        [UnitOfWork]
        public virtual void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
            {
                var users = _userOrganizationUnitRepository.GetAllList(userOu =>
                    userOu.OrganizationUnitId == eventData.Entity.OrganizationUnitId
                );

                foreach (var userOrganizationUnit in users)
                {
                    var cacheKey = userOrganizationUnit.UserId + "@" + (eventData.Entity.TenantId ?? 0);
                    _cacheManager.GetUserPermissionCache().Remove(cacheKey);
                }
            }
        }
    }
}