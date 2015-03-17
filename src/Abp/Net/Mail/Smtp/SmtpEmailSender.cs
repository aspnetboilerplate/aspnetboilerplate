using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.Net.Mail.Smtp
{
    /// <summary>
    /// Implementation of <see cref="IEmailSender"/> to send emails over SMTP.
    /// Uses settings defines in <see cref="AbpEmailSettingProvider"/>.
    /// See <see cref="AbpEmailSettingNames"/> for all available settings.
    /// </summary>
    public class SmtpEmailSender : IEmailSender, ITransientDependency
    {
        private readonly ISmtpClientProvider _smtpClientProvider;
        private readonly ISmtpEmailSenderConfiguration _configuration;

        public SmtpEmailSender(ISmtpClientProvider smtpClientProvider, ISmtpEmailSenderConfiguration configuration)
        {
            _smtpClientProvider = smtpClientProvider;
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(_configuration.DefaultFromAddress, to, subject, body, isBodyHtml);
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
        }

        public async Task SendAsync(MailMessage mail)
        {
            NormalizeMail(mail);

            using (var smtpClient = _smtpClientProvider.CreateSmtpClient())
            {
                await smtpClient.SendMailAsync(mail);                
            }
        }

        private void NormalizeMail(MailMessage mail)
        {
            //TODO: Encodings can be configurable.

            if (mail.From == null || mail.From.Address.IsNullOrEmpty())
            {
                mail.From = new MailAddress(
                    _configuration.DefaultFromAddress,
                    _configuration.DefaultFromDisplayName,
                    Encoding.UTF8
                    );
            }

            if (mail.HeadersEncoding == null)
            {
                mail.HeadersEncoding = Encoding.UTF8;
            }

            if (mail.SubjectEncoding == null)
            {
                mail.SubjectEncoding = Encoding.UTF8;
            }

            if (mail.BodyEncoding == null)
            {
                mail.BodyEncoding = Encoding.UTF8;
            }
        }
    }
}