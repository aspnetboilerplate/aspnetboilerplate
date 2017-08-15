using Abp.Domain.Uow;
using Abp.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserManager_Update_Tests : SampleAppTestBase
    {
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserManager_Update_Tests()
        {
            _userManager = Resolve<UserManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            AbpSession.TenantId = 1; //Default tenant
        }

        [Fact]
        public void Should_Not_Change_Admin_UserName()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var adminUser = _userManager.FindByName("admin");

                adminUser.UserName = "tuana";

                _userManager.Update(adminUser).Succeeded.ShouldBeFalse();

                uow.Complete();
            }
        }
    }
}