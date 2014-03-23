using System;
using System.Net.Mail;
using Abp.Configuration;
using Castle.Core.Logging;

namespace Taskever.Utils.Mail
{
    //TODO: Get setting from configuration
    /// <summary>
    /// Implements <see cref="IEmailService"/> to send emails using current settings.
    /// </summary>
    public class EmailService : IEmailService
    {
        public ILogger Logger { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="EmailService"/>.
        /// </summary>
        public EmailService()
        {
            Logger = NullLogger.Instance;
        }

        public void SendEmail(MailMessage mail)
        {
            try
            {
                mail.From = new MailAddress(SettingHelper.GetSettingValue("Abp.Net.Mail.SenderAddress"), SettingHelper.GetSettingValue("Abp.Net.Mail.DisplayName"));
                using (var client = new SmtpClient(SettingHelper.GetSettingValue("Abp.Net.Mail.ServerAddress"), SettingHelper.GetSettingValue<int>("Abp.Net.Mail.ServerPort")))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(SettingHelper.GetSettingValue("Abp.Net.Mail.Username"), SettingHelper.GetSettingValue("Abp.Net.Mail.Password"));
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not send email!", ex);
            }
        }
    }
}