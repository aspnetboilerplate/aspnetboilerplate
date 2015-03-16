using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Extensions;

namespace Abp.Net.Mail
{
    public class EmailSender : IEmailSender
    {
        private readonly SettingManager _settingManager;

        public EmailSender(SettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            await SendAsync(new MailMessage("...", to, subject, body));
        }

        public async Task SendAsync(MailMessage mail)
        {
            var host = await GetNotEmptySettingValue(AbpEmailSettingNames.Host);
            var userName = await GetNotEmptySettingValue(AbpEmailSettingNames.UserName);
            var password = await GetNotEmptySettingValue(AbpEmailSettingNames.Password);

            var port = await _settingManager.GetSettingValueAsync<int>(AbpEmailSettingNames.Port);
            var domain = await _settingManager.GetSettingValueAsync(AbpEmailSettingNames.Domain);

            //TODO: Add from address, display name and other stuff.

            var smtpClient = new SmtpClient(host, port);
            smtpClient.Credentials = !domain.IsNullOrEmpty()
                ? new NetworkCredential(userName, password, domain)
                : new NetworkCredential(userName, password);

            await smtpClient.SendMailAsync(mail);
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