using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Zero.SampleApp.Users;
using Abp.Zero.SampleApp.Users.Dto;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserAppService_Tests : SampleAppTestBase
    {
        private readonly IUserAppService _userAppService;
        private readonly UserManager _userManager;
        public UserAppService_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
            _userManager = Resolve<UserManager>();
            Resolve<IAuditingConfiguration>().IsEnabledForAnonymousUsers = true;
        }

        [Fact]
        public void Should_Insert_And_Write_Audit_Logs()
        {
            _userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "emre@aspnetboilerplate.com",
                    Name = "Yunus",
                    Surname = "Emre",
                    UserName = "yunus.emre"
                });

            UsingDbContext(
                context =>
                {
                    context.Users.FirstOrDefault(u => u.UserName == "yunus.emre").ShouldNotBe(null);

                    var auditLog = context.AuditLogs.FirstOrDefault();
                    auditLog.ShouldNotBe(null);
                    auditLog.TenantId.ShouldBe(AbpSession.TenantId);
                    auditLog.UserId.ShouldBe(AbpSession.UserId);
                    auditLog.ServiceName.ShouldBe(typeof(UserAppService).FullName);
                    auditLog.MethodName.ShouldBe("CreateUser");
                    auditLog.Exception.ShouldBe(null);
                });
        }

        [Fact]
        public async Task Should_Reset_Password()
        {
            AbpSession.TenantId = 1; //Default tenant   
            var managerUser = await _userManager.FindByNameAsync("manager");
            managerUser.PasswordResetCode = "fc9640bb73ec40a2b42b479610741a5a";
            _userManager.Update(managerUser);

            AbpSession.TenantId = null; //Default tenant  

            await _userAppService.ResetPassword(new ResetPasswordInput
            {
                TenantId = 1,
                UserId = managerUser.Id,
                Password = "123qwe",
                ResetCode = "fc9640bb73ec40a2b42b479610741a5a"
            });

            var updatedUser = UsingDbContext(
                context =>
                {
                    return context.Users.FirstOrDefault(u => u.UserName == "manager");
                });

            updatedUser.UserName.ShouldBe("manager");
            updatedUser.PasswordResetCode.ShouldBe(null);
        }

        [Fact]
        public async Task Should_Change_Security_Stamp()
        {
            _userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "security@stamp.com",
                    Name = "Security",
                    Surname = "Stamp",
                    UserName = "SecurityStamp"
                });

            var firstUser = await _userManager.FindByNameAsync("SecurityStamp");
            var firstSecurityStamp = firstUser.SecurityStamp;

            //change password
            await _userManager.ChangePasswordAsync(firstUser, "asd123");

            User passwordChangedUser = await _userManager.FindByIdAsync(firstUser.Id);
            var passwordChangedUserSecurityStamp = passwordChangedUser.SecurityStamp;
            passwordChangedUserSecurityStamp.ShouldNotBe(firstSecurityStamp);

            //change password2
            var passwordHash = new PasswordHasher().HashPassword("asd123456");
            passwordChangedUser.Password = passwordHash;
            await _userManager.UpdateAsync(passwordChangedUser);

            User passwordChangedUser2 = await _userManager.FindByIdAsync(firstUser.Id);
            var passwordChangedUserSecurityStamp2 = passwordChangedUser2.SecurityStamp;
            passwordChangedUserSecurityStamp2.ShouldNotBe(passwordChangedUserSecurityStamp);

            //update username
            passwordChangedUser.UserName = "SecurityStamp2";
            await _userManager.UpdateAsync(passwordChangedUser);

            User userNameUpdatedUser = await _userManager.FindByIdAsync(firstUser.Id);
            userNameUpdatedUser.ShouldNotBeNull();
            userNameUpdatedUser.UserName.ShouldBe("SecurityStamp2");
            var userNameUpdateUserSecurityStamp = userNameUpdatedUser.SecurityStamp;
            userNameUpdateUserSecurityStamp.ShouldNotBe(passwordChangedUserSecurityStamp2);

            //update email
            userNameUpdatedUser.EmailAddress = "test@hotmail.com";
            await _userManager.UpdateAsync(userNameUpdatedUser);

            User emailUpdatedUser = await _userManager.FindByIdAsync(firstUser.Id);
            emailUpdatedUser.ShouldNotBeNull();
            emailUpdatedUser.EmailAddress.ShouldBe("test@hotmail.com");
            var emailUpdatedUserSecurityStamp = emailUpdatedUser.SecurityStamp;
            emailUpdatedUserSecurityStamp.ShouldNotBe(userNameUpdateUserSecurityStamp);
        }
    }
}
