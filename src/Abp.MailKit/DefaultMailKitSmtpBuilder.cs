using Abp.Dependency;
using Abp.Extensions;
using Abp.Net.Mail.Smtp;
using MailKit.Net.Smtp;

namespace Abp.MailKit
{
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration _smtpEmailSenderConfiguration;

        public DefaultMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration)
        {
            _smtpEmailSenderConfiguration = smtpEmailSenderConfiguration;
        }

        public virtual SmtpClient Build()
        {
            var client = new SmtpClient();

            try
            {
                ConfigureClient(client);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        protected virtual void ConfigureClient(SmtpClient client)
        {
            client.Connect(
                _smtpEmailSenderConfiguration.Host,
                _smtpEmailSenderConfiguration.Port,
                _smtpEmailSenderConfiguration.EnableSsl
            );

            var userName = _smtpEmailSenderConfiguration.UserName;
            if (!userName.IsNullOrEmpty())
            {
                client.Authenticate(
                    _smtpEmailSenderConfiguration.UserName, 
                    _smtpEmailSenderConfiguration.Password
                );
            }
        }
    }
}