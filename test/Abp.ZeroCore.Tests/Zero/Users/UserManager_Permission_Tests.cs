using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users
{
    public class UserManager_Permission_Tests : AbpZeroTestBase
    {
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPermissionManager _permissionManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserManager_Permission_Tests()
        {
            _permissionChecker = Resolve<IPermissionChecker>();
            _permissionManager = Resolve<IPermissionManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _userManager = Resolve<UserManager>();
            _roleManager = Resolve<RoleManager>();
        }

        [Fact]
        public async Task Should_Check_IsGranted_Correctly_When_Logged_In_As_Host_Then_Switched_To_Tenant()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            var defaultTenantId = 1;
            var user = UsingDbContext(defaultTenantId, (context) =>
            {
                return context.Users.Single(f => f.TenantId == defaultTenantId && f.UserName == AbpUserBase.AdminUserName);
            });

            await _userManager.GrantPermissionAsync(user, _permissionManager.GetPermission(AppPermissions.TestPermission));

            var isGranted = await _permissionChecker.IsGrantedAsync(user.ToUserIdentifier(), AppPermissions.TestPermission);
            isGranted.ShouldBe(true);

            // Simulate background jobs
            LoginAsHostAdmin();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    isGranted = await _permissionChecker.IsGrantedAsync(user.ToUserIdentifier(), AppPermissions.TestPermission);
                    isGranted.ShouldBe(true);
                }
            }
        }

        [Fact]
        public async Task Should_Inherit_Permission_From_Role_Of_Organization_Unit()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            // Arrange
            var defaultTenantId = 1;
            var organizationUnit = UsingDbContext(defaultTenantId, (context) =>
            {
                return context.OrganizationUnits.Single(ou => ou.TenantId == defaultTenantId && ou.DisplayName == "OU1");
            });
            var permission = _permissionManager.GetPermission(AppPermissions.TestPermission);

            LoginAsDefaultTenantAdmin();
            using (var uow = _unitOfWorkManager.Begin())
            {
                var role = await _roleManager.GetRoleByNameAsync("MANAGER");
                await _roleManager.GrantPermissionAsync(role, permission);
                await _roleManager.AddToOrganizationUnitAsync(role, organizationUnit);

                await uow.CompleteAsync();
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                var newUser = new User
                {
                    TenantId = AbpSession.TenantId,
                    UserName = "user1",
                    Name = "John",
                    Surname = "Doe",
                    EmailAddress = "user1@aspnetboilerplate.com",
                    IsEmailConfirmed = true,
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                };

                await _userManager.CreateAsync(newUser);

                await uow.CompleteAsync();
            }

            //Actual
            var user = UsingDbContext(defaultTenantId, (context) =>
            {
                return context.Users.Single(u => u.TenantId == defaultTenantId && u.UserName == "user1");
            });

            (await _userManager.IsInRoleAsync(user, "MANAGER")).ShouldBeFalse();
            (await _userManager.IsInOrganizationUnitAsync(user, organizationUnit)).ShouldBeFalse();
            (await _userManager.IsGrantedAsync(user, permission)).ShouldBeFalse();

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _userManager.AddToOrganizationUnitAsync(user, organizationUnit);

                await uow.CompleteAsync();
            }

            //Assert
            (await _userManager.IsInRoleAsync(user, "MANAGER")).ShouldBeTrue();
            (await _userManager.IsInOrganizationUnitAsync(user, organizationUnit)).ShouldBeTrue();
            (await _userManager.IsGrantedAsync(user, permission)).ShouldBeTrue();
        }
    }
}