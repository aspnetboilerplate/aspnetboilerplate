using Abp.Authorization.Roles;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;

namespace Abp.Organizations
{
    /// <summary>
    /// Removes the role from all organization units when a role is deleted.
    /// </summary>
    public class OrganizationUnitRoleRemover : 
        IEventHandler<EntityDeletedEventData<AbpRoleBase>>, 
        ITransientDependency
    {
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrganizationUnitRoleRemover(
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<AbpRoleBase> eventData)
        {
            using (_unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
            {
                _organizationUnitRoleRepository.Delete(
                    uou => uou.RoleId == eventData.Entity.Id
                );
            }
        }
    }
}
