using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;

namespace Abp.Authorization.Roles
{
    /// <summary>
    /// Removes the role from all organization units when a role is deleted.
    /// </summary>
    public class RoleOrganizationUnitRemover : 
        IEventHandler<EntityDeletedEventData<AbpRoleBase>>, 
        ITransientDependency
    {
        private readonly IRepository<RoleOrganizationUnit, long> _roleOrganizationUnitRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public RoleOrganizationUnitRemover(
            IRepository<RoleOrganizationUnit, long> roleOrganizationUnitRepository, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _roleOrganizationUnitRepository = roleOrganizationUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<AbpRoleBase> eventData)
        {
            using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
            {
                _roleOrganizationUnitRepository.Delete(
                    uou => uou.RoleId == eventData.Entity.Id
                );
            }
        }
    }
}
