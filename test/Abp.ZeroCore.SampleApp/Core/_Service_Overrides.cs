using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.MultiTenancy;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Abp.ZeroCore.SampleApp.Core
{
    public class UserManager : AbpUserManager<Role, User>
    {
        public UserManager(AbpRoleManager<Role, User> roleManager, AbpUserStore<Role, User> userStore,
            IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services,
            ILogger<UserManager<User>> logger, IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager, ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<UserRole, long> userRoleRepository, IOrganizationUnitSettings organizationUnitSettings,
            ISettingManager settingManager, IAsyncQueryableExecuter asyncQueryableExecuter) : base(roleManager,
            userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
            services, logger, permissionManager, unitOfWorkManager, cacheManager, organizationUnitRepository,
            userOrganizationUnitRepository, userRoleRepository, organizationUnitSettings, settingManager,
            asyncQueryableExecuter)
        {
        }
    }

    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) :
            base(
                tenantRepository,
                tenantFeatureRepository,
                editionManager,
                featureValueStore)
        {
        }
    }

    public class EditionManager : AbpEditionManager
    {
        public const string DefaultEditionName = "Standard";

        public EditionManager(
            IRepository<Edition> editionRepository,
            IAbpZeroFeatureValueStore featureValueStore)
            : base(
               editionRepository,
               featureValueStore)
        {
        }
    }

    public class RoleManager : AbpRoleManager<Role, User>
    {
        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager> logger,
            IPermissionManager permissionManager,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository
        ) : base(
            store,
            roleValidators,
            keyNormalizer,
            errors,
            logger,
            permissionManager,
            cacheManager,
            unitOfWorkManager,
            roleManagementConfig,
            organizationUnitRepository,
            organizationUnitRoleRepository)
        {
        }
    }

    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        public LogInManager(
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
    }

    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }

    public class FeatureValueStore : AbpFeatureValueStore<Tenant, User>
    {
        public FeatureValueStore(ICacheManager cacheManager,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<EditionFeatureSetting, long> editionFeatureRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                cacheManager,
                tenantFeatureRepository,
                tenantRepository,
                editionFeatureRepository,
                featureManager,
                unitOfWorkManager)
        {

        }
    }

    public class RoleStore : AbpRoleStore<Role, User>
    {
        public RoleStore(IUnitOfWorkManager unitOfWorkManager, IRepository<Role> roleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository,
            IRepository<RoleClaim, long> roleClaimRepository, IAsyncQueryableExecuter asyncQueryableExecuter) : base(
            unitOfWorkManager, roleRepository, rolePermissionSettingRepository, roleClaimRepository,
            asyncQueryableExecuter)
        {
        }
    }

    public class SecurityStampValidator : AbpSecurityStampValidator<Tenant, Role, User>
    {
        public SecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            SignInManager signInManager,
            ISystemClock systemClock,
            ILoggerFactory loggerFactory)
            : base(options, signInManager, systemClock, loggerFactory)
        {
        }
    }

    public class SignInManager : AbpSignInManager<Tenant, Role, User>
    {
        public SignInManager(
            UserManager userManager,
            IHttpContextAccessor contextAccessor,
            UserClaimsPrincipalFactory claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> userConfirmation
        ) : base(
            userManager,
            contextAccessor,
            claimsFactory,
            optionsAccessor,
            logger,
            unitOfWorkManager,
            settingManager,
            schemes,
            userConfirmation)
        {
        }
    }

    public class UserStore : AbpUserStore<Role, User>
    {
        public UserStore(IUnitOfWorkManager unitOfWorkManager, IRepository<User, long> userRepository,
            IRepository<Role> roleRepository, IAsyncQueryableExecuter asyncQueryableExecuter,
            IRepository<UserRole, long> userRoleRepository, IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserClaim, long> userClaimRepository, IRepository<UserToken, long> userTokenRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository) : base(unitOfWorkManager,
            userRepository, roleRepository, asyncQueryableExecuter, userRoleRepository, userLoginRepository,
            userClaimRepository, userTokenRepository, userPermissionSettingRepository, userOrganizationUnitRepository,
            organizationUnitRoleRepository)
        {
        }
    }

    public class UserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory<User, Role>
    {
        public UserClaimsPrincipalFactory(
            UserManager userManager,
            RoleManager roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(
                userManager,
                roleManager,
                optionsAccessor)
        {
        }

        [UnitOfWork]
        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            return await base.CreateAsync(user);
        }
    }
}
