using System.Net.Mail;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Abp.MailKit.Tests
{
    public class MailKitEmailSender_Tests
    {
        //[Fact]  //Need to set configuration before executing this test
        public void ShouldSend()
        {
            var mailSender = CreateMailKitEmailSender();

            mailSender.Send("from_mail_address", "to_mail_address", "subject", "body", true);
        }

        //[Fact]  //Need to set configuration before executing this test
        public async Task ShouldSendAsync()
        {
            var mailSender = CreateMailKitEmailSender();

            await mailSender.SendAsync("from_mail_address", "to_mail_address", "subject", "body", true);
        }

        //[Fact]  //Need to set configuration before executing this test
        public async Task ShouldSendMailMessageAsync()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("from_mail_address", "to_mail_address", "subject", "body")
            { IsBodyHtml = true };

            await mailSender.SendAsync(mailMessage);
        }

        //[Fact]  //Need to set configuration before executing this test
        public void ShouldSendMailMessage()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("from_mail_address", "to_mail_address", "subject", "body")
            { IsBodyHtml = true };

            mailSender.Send(mailMessage);
        }

        //[Fact]  //Need to set configuration before executing this test
        public void ShouldSendWithDefaultFrom()
        {
            var mailSender = CreateMailKitEmailSender();

            mailSender.Send("to_mail_address", "subject", "body", true);
        }

        private static MailKitEmailSender CreateMailKitEmailSender()
        {
            var mailConfig = Substitute.For<IAbpMailKitConfiguration>();

            mailConfig.DefaultFromAddress.Returns("...");
            mailConfig.DefaultFromDisplayName.Returns("...");

            mailConfig.Host.Returns("stmp_server_name");
            mailConfig.Port.Returns(587);
            mailConfig.EnableSsl.Returns(false);

            //configuration.Domain.Returns("...");
            mailConfig.UserName.Returns("mail_server_user_name");
            mailConfig.Password.Returns("mail_server_password.");

            // MailKit specifics
            //mailConfig.SecureSocketOption.Returns((MailKit.Security.SecureSocketOptions?)null);
            mailConfig.CheckCertificateRevocation.Returns((bool?)null);
            mailConfig.DisableCertificateValidation.Returns((bool?)null);

            var mailSender = new MailKitEmailSender(mailConfig, new DefaultMailKitSmtpBuilder(mailConfig));
            return mailSender;
        }
    }
}
