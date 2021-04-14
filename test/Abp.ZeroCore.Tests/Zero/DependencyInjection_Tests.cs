using System;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.ZeroCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using Xunit;

using SecurityStampValidator = Abp.ZeroCore.SampleApp.Core.SecurityStampValidator;

namespace Abp.Zero
{
    public class DependencyInjection_Tests : AbpZeroTestBase
    {
        [Fact]
        public void Should_Resolve_UserManager()
        {
            LocalIocManager.Resolve<UserManager<User>>();
            LocalIocManager.Resolve<AbpUserManager<Role, User>>();
            LocalIocManager.Resolve<UserManager>();
        }

        [Fact]
        public void Should_Resolve_RoleManager()
        {
            LocalIocManager.Resolve<RoleManager<Role>>();
            LocalIocManager.Resolve<AbpRoleManager<Role, User>>();
            LocalIocManager.Resolve<RoleManager>();
        }

        [Fact]
        public void Should_Resolve_SignInManager()
        {
            LocalIocManager.Resolve<SignInManager<User>>();
            LocalIocManager.Resolve<AbpSignInManager<Tenant, Role, User>>();
            LocalIocManager.Resolve<SignInManager>();
        }

        [Fact]
        public void Should_Resolve_LoginManager()
        {
            LocalIocManager.Resolve<AbpLogInManager<Tenant, Role, User>>();
            LocalIocManager.Resolve<LogInManager>();
        }

        [Fact]
        public void Should_Resolve_SecurityStampValidator()
        {
            LocalIocManager.Resolve<AbpSecurityStampValidator<Tenant, Role, User>>();
            LocalIocManager.Resolve<SecurityStampValidator<User>>();
            LocalIocManager.Resolve<SecurityStampValidator>();
        }

        [Fact]
        public void Should_Resolve_UserClaimsPrincipalFactory()
        {
            LocalIocManager.Resolve<UserClaimsPrincipalFactory<User, Role>>();
            LocalIocManager.Resolve<AbpUserClaimsPrincipalFactory<User, Role>>();
            LocalIocManager.Resolve<IUserClaimsPrincipalFactory<User>>();
            LocalIocManager.Resolve<UserClaimsPrincipalFactory>();
        }

        [Fact]
        public void Should_Resolve_TenantManager()
        {
            LocalIocManager.Resolve<AbpTenantManager<Tenant, User>>();
            LocalIocManager.Resolve<TenantManager>();
        }

        [Fact]
        public void Should_Resolve_EditionManager()
        {
            LocalIocManager.Resolve<AbpEditionManager>();
            LocalIocManager.Resolve<EditionManager>();
        }

        [Fact]
        public void Should_Resolve_PermissionChecker()
        {
            LocalIocManager.Resolve<IPermissionChecker>();
            LocalIocManager.Resolve<PermissionChecker<Role, User>>();
            LocalIocManager.Resolve<PermissionChecker>();
        }

        [Fact]
        public void Should_Resolve_FeatureValueStore()
        {
            LocalIocManager.Resolve<IFeatureValueStore>();
            LocalIocManager.Resolve<AbpFeatureValueStore<Tenant, User>>();
            LocalIocManager.Resolve<FeatureValueStore>();
        }

        [Fact]
        public void Should_Resolve_UserStore()
        {
            LocalIocManager.Resolve<IUserStore<User>>();
            LocalIocManager.Resolve<AbpUserStore<Role, User>>();
            LocalIocManager.Resolve<UserStore>();
        }

        [Fact]
        public void Should_Resolve_RoleStore()
        {
            LocalIocManager.Resolve<IRoleStore<Role>>();
            LocalIocManager.Resolve<AbpRoleStore<Role, User>>();
            LocalIocManager.Resolve<RoleStore>();
        }

        [Fact]
        public void Should_Resolve_LazyRoleStore()
        {
            LocalIocManager.Resolve<Lazy<RoleStore>>();
        }
    }
}
