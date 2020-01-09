using System.Linq;
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
    }
}
