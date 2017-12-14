using System.Threading.Tasks;
using Abp.Authorization;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;
using System.Linq;
using Abp.Authorization.Users;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Application;

namespace Abp.Zero.Users
{
    public class UserManager_Permission_Tests : AbpZeroTestBase
    {
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPermissionManager _permissionManager;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserManager_Permission_Tests()
        {
            _permissionChecker = Resolve<IPermissionChecker>();
            _permissionManager = Resolve<IPermissionManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _userManager = Resolve<UserManager>();
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
    }
}