using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using Abp.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Roles
{
    public class RoleManager_StaticRole_Tests : SampleAppTestBase
    {
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly IPermissionManager _permissionManager;

        public RoleManager_StaticRole_Tests()
        {
            _tenantManager = Resolve<TenantManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _roleManagementConfig = Resolve<IRoleManagementConfig>();
            _permissionManager = Resolve<IPermissionManager>();
        }

        private async Task CreateTestStaticRoles()
        {
            _roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    "admin",
                    MultiTenancySides.Host)
                {
                    GrantedPermissions = { "Permission5" }
                }
            );

            _roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    "admin",
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: true)
            );

            _roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    "supporter",
                    MultiTenancySides.Tenant)
                {
                    GrantedPermissions = { "Permission1", "Permission2" }
                }
            );

            using (var uow = _unitOfWorkManager.Begin())
            {
                var tenant = new Tenant("Tenant1", "Tenant1");
                await _tenantManager.CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync();

                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    AbpSession.TenantId = tenant.Id;

                    await RoleManager.CreateStaticRoles(tenant.Id);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Static_Roles_Can_Have_Default_Granted_Permissions()
        {
            await CreateTestStaticRoles();

            var tenant = GetTenant("Tenant1");
            AbpSession.TenantId = tenant.Id;

            var adminRole = await RoleManager.GetRoleByNameAsync("admin");
            var supporterRole = await RoleManager.GetRoleByNameAsync("supporter");

            //Default granted permissions

            (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission1")).ShouldBe(true);
            (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission2")).ShouldBe(true);
            (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission3")).ShouldBe(true);
            (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission4")).ShouldBe(true);
            (await RoleManager.IsGrantedAsync(adminRole.Id, "Permission5")).ShouldBe(false);
            (await RoleManager.IsGrantedAsync(adminRole.Id, "FirstLevelChilPermission1")).ShouldBe(true);
            (await RoleManager.IsGrantedAsync(adminRole.Id, "SecondLevelChildPermission1")).ShouldBe(true);
            
            (await RoleManager.IsGrantedAsync(supporterRole.Id, "Permission1")).ShouldBe(true);
            (await RoleManager.IsGrantedAsync(supporterRole.Id, "Permission2")).ShouldBe(true);
            (await RoleManager.IsGrantedAsync(supporterRole.Id, "Permission3")).ShouldBe(false);
            (await RoleManager.IsGrantedAsync(supporterRole.Id, "Permission4")).ShouldBe(false);

            //Grant new permission that is not granted by default
            await RoleManager.GrantPermissionAsync(supporterRole, _permissionManager.GetPermission("Permission3"));
            (await RoleManager.IsGrantedAsync(supporterRole.Id, "Permission3")).ShouldBe(true);

            //Prohibit a permission that is granted by default
            await RoleManager.ProhibitPermissionAsync(supporterRole, _permissionManager.GetPermission("Permission1"));
            (await RoleManager.IsGrantedAsync(supporterRole.Id, "Permission1")).ShouldBe(false);
        }
    }
}
