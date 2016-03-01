using Adorable.Net.Mail;
using Adorable.Net.Mail.Smtp;
using Xunit;

namespace Adorable.TestBase.SampleApplication.Tests.Net.Mail
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
