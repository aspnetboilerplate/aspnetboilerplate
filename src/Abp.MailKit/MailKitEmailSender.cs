using System.Threading.Tasks;
using Abp.Net.Mail;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Net.Mail;

namespace Abp.MailKit
{
    public class MailKitEmailSender : EmailSenderBase
    {
        private readonly IMailKitSmtpBuilder _smtpBuilder;

        public MailKitEmailSender(
            IEmailSenderConfiguration smtpEmailSenderConfiguration,
            IMailKitSmtpBuilder smtpBuilder)
            : base(
                  smtpEmailSenderConfiguration)
        {
            _smtpBuilder = smtpBuilder;
        }

        public override async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            using (var client = BuildSmtpClient())
            {
                var message = BuildMimeMessage(from, to, subject, body, isBodyHtml);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public override void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            using (var client = BuildSmtpClient())
            {
                var message = BuildMimeMessage(from, to, subject, body, isBodyHtml);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        protected override async Task SendEmailAsync(MailMessage mail)
        {
            using (var client = BuildSmtpClient())
            {
                var message = MimeMessage.CreateFromMailMessage(mail);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        protected override void SendEmail(MailMessage mail)
        {
            using (var client = BuildSmtpClient())
            {
                var message = MimeMessage.CreateFromMailMessage(mail);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        protected virtual SmtpClient BuildSmtpClient()
        {
            return _smtpBuilder.Build();
        }

        private static MimeMessage BuildMimeMessage(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            var bodyType = isBodyHtml ? "html" : "plain";
            var message = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart(bodyType)
                {
                    Text = body
                }
            };

            message.From.Add(new MailboxAddress(from));
            message.To.Add(new MailboxAddress(to));
            
            return message;
        }
    }
}