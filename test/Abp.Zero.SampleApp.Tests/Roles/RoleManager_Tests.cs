using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Roles
{
    public class RoleManager_Tests : SampleAppTestBase
    {
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRoleManagementConfig _roleManagementConfig;

        public RoleManager_Tests()
        {
            _tenantManager = Resolve<TenantManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _roleManagementConfig = Resolve<IRoleManagementConfig>();
        }

        [Fact]
        public async Task Should_Create_And_Retrieve_Role()
        {
            await CreateRole("Role1");

            var role1Retrieved = await RoleManager.FindByNameAsync("Role1");
            role1Retrieved.ShouldNotBe(null);
            role1Retrieved.Name.ShouldBe("Role1");
        }

        [Fact]
        public async Task Should_Not_Create_For_Duplicate_Name_Or_DisplayName()
        {
            //Create a role and check
            await CreateRole("Role1", "Role One");
            (await RoleManager.FindByNameAsync("Role1")).ShouldNotBe(null);

            //Create with same name
            (await RoleManager.CreateAsync(new Role(null, "Role1", "Role Uno"))).Succeeded.ShouldBe(false);
            (await RoleManager.CreateAsync(new Role(null, "Role2", "Role One"))).Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task PermissionTests()
        {
            var role1 = await CreateRole("Role1");

            (await RoleManager.IsGrantedAsync(role1.Id, PermissionManager.GetPermission("Permission1"))).ShouldBe(false);
            (await RoleManager.IsGrantedAsync(role1.Id, PermissionManager.GetPermission("Permission3"))).ShouldBe(false);

            await GrantPermissionAsync(role1, "Permission1");
            await ProhibitPermissionAsync(role1, "Permission1");
            await ProhibitPermissionAsync(role1, "Permission3");
            await GrantPermissionAsync(role1, "Permission3");
            await GrantPermissionAsync(role1, "Permission1");
            await ProhibitPermissionAsync(role1, "Permission3");

            var grantedPermissions = await RoleManager.GetGrantedPermissionsAsync(role1);
            grantedPermissions.Count.ShouldBe(1);
            grantedPermissions.ShouldContain(p => p.Name == "Permission1");

            var newPermissionList = new List<Permission>
                                    {
                                        PermissionManager.GetPermission("Permission1"),
                                        PermissionManager.GetPermission("Permission2"),
                                        PermissionManager.GetPermission("Permission3")
                                    };

            await RoleManager.SetGrantedPermissionsAsync(role1, newPermissionList);

            grantedPermissions = await RoleManager.GetGrantedPermissionsAsync(role1);

            grantedPermissions.Count.ShouldBe(3);
            grantedPermissions.ShouldContain(p => p.Name == "Permission1");
            grantedPermissions.ShouldContain(p => p.Name == "Permission2");
            grantedPermissions.ShouldContain(p => p.Name == "Permission3");
        }

        [Fact]
        public async Task PermissionWithFeatureDependencyTests()
        {
            _roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    "admin",
                    MultiTenancySides.Tenant)
            );

            Tenant tenant;
            Role adminRole;

            using (var uow = _unitOfWorkManager.Begin())
            {
                tenant = new Tenant("Tenant1", "Tenant1");
                await _tenantManager.CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync();

                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    AbpSession.TenantId = tenant.Id;

                    await RoleManager.CreateStaticRoles(tenant.Id);
                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    adminRole = RoleManager.Roles.Single(r => r.Name == "admin");
                    await RoleManager.GrantAllPermissionsAsync(adminRole);
                }

                await uow.CompleteAsync();
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    AbpSession.TenantId = tenant.Id;

                    (await RoleManager.IsGrantedAsync(adminRole.Id, "PermissionWithFeatureDependency")).ShouldBe(false);
                    (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission1")).ShouldBe(true);
                    (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission2")).ShouldBe(true);
                    (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission3")).ShouldBe(true);
                    (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission4")).ShouldBe(true);

                    var grantedPermissions = await RoleManager.GetGrantedPermissionsAsync(adminRole);

                    grantedPermissions.Count.ShouldBe(6);
                    grantedPermissions.ShouldContain(p => p.Name == "Permission1");
                    grantedPermissions.ShouldContain(p => p.Name == "Permission2");
                    grantedPermissions.ShouldContain(p => p.Name == "Permission3");
                    grantedPermissions.ShouldContain(p => p.Name == "Permission4");
                }

                await uow.CompleteAsync();
            }
        }
    }
}