using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using ModuleZeroSampleProject.Authorization;
using ModuleZeroSampleProject.MultiTenancy;

namespace ModuleZeroSampleProject.Users
{
    public class UserManager : AbpUserManager<Tenant, Role, User>
    {
        public UserManager(
            UserStore userStore, 
            RoleManager roleManager, 
            IRepository<Tenant> tenantRepository, 
            IMultiTenancyConfig multiTenancyConfig, 
            IPermissionManager permissionManager, 
            IUnitOfWorkManager unitOfWorkManager, 
            ISettingManager settingManager, 
            IUserManagementConfig userManagementConfig, 
            IIocResolver iocResolver,
            Abp.Runtime.Caching.ICacheManager cacheManager)
            : base(
                userStore, 
                roleManager, 
                tenantRepository, 
                multiTenancyConfig, 
                permissionManager, 
                unitOfWorkManager, 
                settingManager, 
                userManagementConfig, 
                iocResolver,
            cacheManager)
        {
        }
    }
}
