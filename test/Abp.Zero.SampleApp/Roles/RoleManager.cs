using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.Roles
{
    public class RoleManager : AbpRoleManager<Role, User>
    {
        public RoleManager(
            RoleStore store, 
            IPermissionManager permissionManager, 
            IRoleManagementConfig roleManagementConfig, 
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
            store, 
            permissionManager, 
            roleManagementConfig, 
            cacheManager,
            unitOfWorkManager)
        {
        }
    }
}