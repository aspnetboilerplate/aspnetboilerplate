using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Zero;
using Abp.Zero.Configuration;
using Abp.ZeroCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;

namespace Abp.ZeroCore.SampleApp.Authorization;

public class AppLogInManager : AbpLogInManager<Tenant, Role, User>
{
    public AppLogInManager(
        AbpUserManager<Role, User> userManager,
        IMultiTenancyConfig multiTenancyConfig,
        IRepository<Tenant> tenantRepository,
        IUnitOfWorkManager unitOfWorkManager,
        ISettingManager settingManager,
        IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
        IUserManagementConfig userManagementConfig,
        IIocResolver iocResolver,
        IPasswordHasher<User> passwordHasher,
        AbpRoleManager<Role, User> roleManager,
        UserClaimsPrincipalFactory claimsPrincipalFactory
    ) : base(
        userManager,
        multiTenancyConfig,
        tenantRepository,
        unitOfWorkManager,
        settingManager,
        userLoginAttemptRepository,
        userManagementConfig,
        iocResolver,
        passwordHasher,
        roleManager,
        claimsPrincipalFactory)
    {
    }

    protected override async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternal(string userNameOrEmailAddress, string plainPassword, string tenancyName, bool shouldLockout)
    {
        if (userNameOrEmailAddress == "forbidden-user")
        {
            var result = new AbpLoginResult<Tenant, User>(AbpLoginResultType.FailedForOtherReason);
            result.SetFailReason(new LocalizableString("ForbiddenUser", AbpZeroConsts.LocalizationSourceName));
            return result;
        }

        return await base.LoginAsyncInternal(userNameOrEmailAddress, plainPassword, tenancyName, shouldLockout);
    }
}