using System;
using Abp.Net.Mail.Smtp;
using Abp.Extensions;
using MailKit.Net.Smtp;

namespace Abp.MailKit
{
    public class AbpMailKitConfiguration : IAbpMailKitConfiguration
    {
        public Action<SmtpClient> SmtpClientConfigurer { get; set; }

        public AbpMailKitConfiguration(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration)
        {
            SmtpClientConfigurer = client =>
            {
                client.Connect(smtpEmailSenderConfiguration.Host, smtpEmailSenderConfiguration.Port, smtpEmailSenderConfiguration.EnableSsl);

                var userName = smtpEmailSenderConfiguration.UserName;
                if (!userName.IsNullOrEmpty())
                {
                    client.Authenticate(smtpEmailSenderConfiguration.UserName, smtpEmailSenderConfiguration.Password);
                }
            };
        }
    }
}