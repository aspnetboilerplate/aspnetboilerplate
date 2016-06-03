using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Net.Mail
{
    public class SmtpEmailSender_Resolve_Test : AbpIntegratedTestBase
    {
        [Fact]
        public void Should_Resolve_EmailSenders()
        {
            Resolve<IEmailSender>();
            Resolve<ISmtpEmailSender>();
        }
    }
}
