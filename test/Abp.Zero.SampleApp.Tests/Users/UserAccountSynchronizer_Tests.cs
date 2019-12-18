using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Timing;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Users;
using Abp.Zero.SampleApp.Users.Dto;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserAccountSynchronizer_Tests : SampleAppTestBase
    {
        private readonly IUserAppService _userAppService;
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserAccountSynchronizer_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
            _tenantManager = Resolve<TenantManager>();
            _userManager = Resolve<UserManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void Should_Create_UserAccount_When_User_Created()
        {
            var user = CreateAndGetUser();

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.FirstOrDefault(u => u.UserName == "yunus.emre");
                    userAccount.ShouldNotBe(null);
                    userAccount.UserId.ShouldBe(user.Id);
                    userAccount.TenantId.ShouldBe(user.TenantId);
                    userAccount.UserName.ShouldBe(user.UserName);
                    userAccount.EmailAddress.ShouldBe(user.EmailAddress);
                });
        }

        [Fact]
        public void Should_Update_UserAccount_When_User_Updated()
        {
            var user = CreateAndGetUser();
            var now = Clock.Now;
            _userAppService.UpdateUser(new UpdateUserInput
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                UserName = "y.emre",
                EmailAddress = "y.emre@aspnetboilerplate.com",
                LastLoginTime = now
            });

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.FirstOrDefault(u => u.UserName == "y.emre");
                    userAccount.ShouldNotBe(null);
                    userAccount.UserId.ShouldBe(user.Id);
                    userAccount.TenantId.ShouldBe(user.TenantId);
                    userAccount.UserName.ShouldBe("y.emre");
                    userAccount.EmailAddress.ShouldBe("y.emre@aspnetboilerplate.com");
                });
        }

        [Fact]
        public void Should_Delete_UserAccount_When_User_Deleted()
        {
            var user = CreateAndGetUser();

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == "yunus.emre");
                    userAccount.IsDeleted.ShouldBe(false);
                });

            _userAppService.DeleteUser(user.Id);

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == "yunus.emre");
                    userAccount.IsDeleted.ShouldBe(true);
                });
        }

        [Fact]
        public async Task Should_Delete_UserAccount_When_Tenant_Deleted()
        {
            var tenant = new Tenant("test-tenant-name", "test-tenant-name");
            await _tenantManager.CreateAsync(tenant);

            var user = new User
            {
                TenantId = tenant.Id,
                UserName = "TestTenantUser",
                Name = "TestTenantUser",
                Surname = "TestTenantUser",
                EmailAddress = "TestTenantUser@abp.io",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            };

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _userManager.CreateAsync(user);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                await uow.CompleteAsync();
            }

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == "TestTenantUser");
                    userAccount.IsDeleted.ShouldBe(false);
                });

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _tenantManager.DeleteAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                await uow.CompleteAsync();
            }

            UsingDbContext(
                context =>
                {
                    var userAccount = context.UserAccounts.First(u => u.UserName == "TestTenantUser");
                    userAccount.IsDeleted.ShouldBe(true);
                });
        }

        private User CreateAndGetUser()
        {
            _userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "emre@aspnetboilerplate.com",
                    Name = "Yunus",
                    Surname = "Emre",
                    UserName = "yunus.emre"
                });

            return UsingDbContext(
                context =>
                {
                    return context.Users.First(u => u.UserName == "yunus.emre");
                });
        }
    }
}
