using System;
#if NET46
using System.Net.Mail;
#endif
using System.Threading.Tasks;
using Castle.Core.Logging;
using Abp.Threading;

namespace Abp.Net.Mail
{
    /// <summary>
    /// This class is an implementation of <see cref="IEmailSender"/> as similar to null pattern.
    /// It does not send emails but logs them.
    /// </summary>
    public class NullEmailSender : EmailSenderBase
    {
        public ILogger Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="NullEmailSender"/> object.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public NullEmailSender(IEmailSenderConfiguration configuration)
            : base(configuration)
        {
            Logger = NullLogger.Instance;
        }

#if NET46
        protected override Task SendEmailAsync(MailMessage mail)
        {
            Logger.Warn("USING NullEmailSender!");
            Logger.Debug("SendEmailAsync:");
            LogEmail(mail);
            return Task.FromResult(0);
        }

        protected override void SendEmail(MailMessage mail)
        {
            Logger.Warn("USING NullEmailSender!");
            Logger.Debug("SendEmail:");
            LogEmail(mail);
        }

        private void LogEmail(MailMessage mail)
        {
            Logger.Debug(mail.To.ToString());
            Logger.Debug(mail.CC.ToString());
            Logger.Debug(mail.Subject);
            Logger.Debug(mail.Body);
        }
#else
        public override Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            Send(from, to, subject, body, isBodyHtml);
            return AbpTaskCache.CompletedTask;
        }

        public override void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            Logger.Debug("from       : " + from);
            Logger.Debug("to         : " + to);
            Logger.Debug("subject    : " + subject);
            Logger.Debug("body       : " + body);
            Logger.Debug("isBodyHtml : " + isBodyHtml.ToString());
        }
#endif
    }
}