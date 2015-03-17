using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.Net.Mail
{
    //TODO: Encodings can be configurable.
    /// <summary>
    /// Implementation of <see cref="IEmailSender"/> to send emails over SMTP.
    /// Uses settings defines in <see cref="AbpEmailSettingProvider"/>.
    /// See <see cref="AbpEmailSettingNames"/> for all available settings.
    /// </summary>
    public class SmtpEmailSender : IEmailSender, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public SmtpEmailSender(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(await GetNotEmptySettingValue(AbpEmailSettingNames.DefaultFromAddress), to, subject, body, isBodyHtml);
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
        }

        public async Task SendAsync(MailMessage mail)
        {
            await NormalizeMail(mail);
            var smtpClient = await CreateSmtpClientAsync();
            await smtpClient.SendMailAsync(mail);
        }

        private async Task NormalizeMail(MailMessage mail)
        {
            if (mail.From == null || mail.From.Address.IsNullOrEmpty())
            {
                mail.From = new MailAddress(
                    await GetNotEmptySettingValue(AbpEmailSettingNames.DefaultFromAddress),
                    await _settingManager.GetSettingValueAsync(AbpEmailSettingNames.DefaultFromDisplayName),
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

        private async Task<SmtpClient> CreateSmtpClientAsync()
        {
            var host = await GetNotEmptySettingValue(AbpEmailSettingNames.Host);
            var port = await _settingManager.GetSettingValueAsync<int>(AbpEmailSettingNames.Port);

            var smtpClient = new SmtpClient(host, port);

            if (await _settingManager.GetSettingValueAsync<bool>(AbpEmailSettingNames.EnableSsl))
            {
                smtpClient.EnableSsl = true;
            }

            if (await _settingManager.GetSettingValueAsync<bool>(AbpEmailSettingNames.UseDefaultCredentials))
            {
                smtpClient.UseDefaultCredentials = true;
            }
            else
            {
                smtpClient.UseDefaultCredentials = false;

                var userName = await GetNotEmptySettingValue(AbpEmailSettingNames.UserName);
                var password = await GetNotEmptySettingValue(AbpEmailSettingNames.Password);
                var domain = await _settingManager.GetSettingValueAsync(AbpEmailSettingNames.Domain);

                smtpClient.Credentials = !domain.IsNullOrEmpty()
                    ? new NetworkCredential(userName, password, domain)
                    : new NetworkCredential(userName, password);
            }

            return smtpClient;
        }

        private async Task<string> GetNotEmptySettingValue(string name)
        {
            var value = await _settingManager.GetSettingValueAsync(name);
            if (value.IsNullOrEmpty())
            {
                throw new AbpException("No setting value defined for: " + name);
            }

            return value;
        }
    }
}