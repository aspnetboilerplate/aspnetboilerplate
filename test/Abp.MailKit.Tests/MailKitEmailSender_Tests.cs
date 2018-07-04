using System.Threading.Tasks;
using Abp.Net.Mail.Smtp;
using NSubstitute;
using System.Net.Mail;

namespace Abp.MailKit.Tests
{
    public class MailKitEmailSender_Tests
    {
        //[Fact]
        public void ShouldSend()
        {
            var mailSender = CreateMailKitEmailSender();

            mailSender.Send("from_mail_address", "to_mail_address", "subject", "body", true);
        }

        //[Fact]
        public async Task ShouldSendAsync()
        {
            var mailSender = CreateMailKitEmailSender();

            await mailSender.SendAsync("from_mail_address", "to_mail_address", "subject", "body", true);
        }

        //[Fact]
        public async Task ShouldSendMailMessageAsync()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("from_mail_address", "to_mail_address", "subject", "body")
            { IsBodyHtml = true };

            await mailSender.SendAsync(mailMessage);
        }

        //[Fact]
        public void ShouldSendMailMessage()
        {
            var mailSender = CreateMailKitEmailSender();
            var mailMessage = new MailMessage("from_mail_address", "to_mail_address", "subject", "body")
            { IsBodyHtml = true };

            mailSender.Send(mailMessage);
        }

        private static MailKitEmailSender CreateMailKitEmailSender()
        {
            var mailConfig = Substitute.For<ISmtpEmailSenderConfiguration>();

            mailConfig.Host.Returns("stmp_server_name");
            mailConfig.UserName.Returns("mail_server_user_name");
            mailConfig.Password.Returns("mail_server_password");
            mailConfig.Port.Returns(587);
            mailConfig.EnableSsl.Returns(false);

            var mailSender = new MailKitEmailSender(mailConfig, new DefaultMailKitSmtpBuilder(mailConfig, new AbpMailKitConfiguration()));
            return mailSender;
        }
    }
}
