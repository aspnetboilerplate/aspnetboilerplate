using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Net.Mail
{
    public class SmtpEmailSender_Resolve_Test : AbpIntegratedTestBase<AbpKernelModule>
    {
        [Fact]
        public void Should_Resolve_EmailSenders()
        {
            Resolve<IEmailSender>().ShouldBeOfType(typeof(SmtpEmailSender));
            Resolve<ISmtpEmailSender>().ShouldBeOfType(typeof(SmtpEmailSender));
        }
    }
}
