using Abp.Dependency;
using Abp.Net.Mail.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Abp.MailKit
{
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration _smtpEmailSenderConfiguration;
        private readonly IAbpMailKitConfiguration _abpMailKitConfiguration;

        public DefaultMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration, IAbpMailKitConfiguration abpMailKitConfiguration)
        {
            _smtpEmailSenderConfiguration = smtpEmailSenderConfiguration;
            _abpMailKitConfiguration = abpMailKitConfiguration;
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
                GetSecureSocketOption()
            );

            if (_smtpEmailSenderConfiguration.UseDefaultCredentials)
            {
                return;
            }

            client.Authenticate(
                _smtpEmailSenderConfiguration.UserName,
                _smtpEmailSenderConfiguration.Password
            );
        }

        protected virtual SecureSocketOptions GetSecureSocketOption()
        {
            if (_abpMailKitConfiguration.SecureSocketOption.HasValue)
            {
                return _abpMailKitConfiguration.SecureSocketOption.Value;
            }

            return _smtpEmailSenderConfiguration.EnableSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;
        }
    }
}