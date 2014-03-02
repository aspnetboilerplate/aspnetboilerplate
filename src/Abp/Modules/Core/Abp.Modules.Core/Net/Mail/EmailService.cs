using System;
using System.Configuration;
using System.Net.Mail;
using Abp.Configuration;
using Castle.Core.Logging;

namespace Abp.Net.Mail
{
    //TODO: Get setting from configuration
    /// <summary>
    /// Implements <see cref="IEmailService"/> to send emails using current settings.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ISettingValueManager _settingValueManager;

        public ILogger Logger { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="EmailService"/>.
        /// </summary>
        public EmailService(ISettingValueManager settingValueManager)
        {
            _settingValueManager = settingValueManager;
            Logger = NullLogger.Instance;
        }

        public void SendEmail(MailMessage mail)
        {
            try
            {
                mail.From = new MailAddress(_settingValueManager.GetSettingValue("Email.SenderAddress"), _settingValueManager.GetSettingValue("Email.DisplayName"));
                using (var client = new SmtpClient(_settingValueManager.GetSettingValue("Email.Server"), _settingValueManager.GetSettingValue<int>("Email.Port")))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_settingValueManager.GetSettingValue("Email.SenderAddress"), _settingValueManager.GetSettingValue("Email.SenderPassword"));
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not send email!", ex);
            }
        }

        //public void SendEmail(MailMessage mail)
        //{
        //    try
        //    {
        //        mail.From = new MailAddress(ConfigurationManager.AppSettings["Email.SenderAddress"], "Taskever");
        //        using (var client = new SmtpClient(ConfigurationManager.AppSettings["Email.Server"], 587))
        //        {
        //            client.UseDefaultCredentials = false;
        //            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email.SenderAddress"], ConfigurationManager.AppSettings["Email.SenderPassword"]);
        //            client.Send(mail);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Warn("Can not send email!", ex);
        //    }
        //}
    }
}