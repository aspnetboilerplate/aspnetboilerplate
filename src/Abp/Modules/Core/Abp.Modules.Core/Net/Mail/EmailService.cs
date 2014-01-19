using System;
using System.Configuration;
using System.Net.Mail;
using Castle.Core.Logging;

namespace Abp.Net.Mail
{
    public class EmailService : IEmailService
    {
        public ILogger Logger { get; set; }

        public void SendEmail(MailMessage mail)
        {
            try
            {
                mail.From = new MailAddress(ConfigurationManager.AppSettings["Email.SenderAddress"], "Taskever");
                using (var client = new SmtpClient(ConfigurationManager.AppSettings["Email.Server"], 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email.SenderAddress"], ConfigurationManager.AppSettings["Email.SenderPassword"]);
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Can not send email!", ex);
            }
        }
    }
}