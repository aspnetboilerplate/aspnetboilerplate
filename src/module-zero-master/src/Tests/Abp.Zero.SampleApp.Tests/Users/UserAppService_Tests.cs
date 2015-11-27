using System.Linq;
using Abp.Auditing;
using Abp.Zero.SampleApp.Users;
using Abp.Zero.SampleApp.Users.Dto;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserAppService_Tests : SampleAppTestBase
    {
        private readonly IUserAppService _userAppService;

        public UserAppService_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
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
    }
}
