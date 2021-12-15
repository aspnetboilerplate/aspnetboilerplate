using Abp.Dependency;
using Abp.Net.Mail.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Abp.MailKit
{
    public class DefaultMailKitSmtpBuilder : IMailKitSmtpBuilder, ITransientDependency
    {
        private readonly IAbpMailKitConfiguration _abpMailKitConfiguration;

        public DefaultMailKitSmtpBuilder(IAbpMailKitConfiguration abpMailKitConfiguration)
        {
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
            if (_abpMailKitConfiguration.DisableCertificateValidation.GetValueOrDefault(false))
            {
                client.ServerCertificateValidationCallback = (mysender, certificate, chain, sslPolicyErrors) => { return true; };
            }


            if (_abpMailKitConfiguration.CheckCertificateRevocation.HasValue)
            {
                client.CheckCertificateRevocation = _abpMailKitConfiguration.CheckCertificateRevocation.Value;
            }

            client.Connect(
                _abpMailKitConfiguration.Host,
                _abpMailKitConfiguration.Port,
                GetSecureSocketOption()
            );

            if (_abpMailKitConfiguration.UseDefaultCredentials)
            {
                return;
            }

            client.Authenticate(
                _abpMailKitConfiguration.UserName,
                _abpMailKitConfiguration.Password
            );
        }

        protected virtual SecureSocketOptions GetSecureSocketOption()
        {
            if (_abpMailKitConfiguration.SecureSocketOption.HasValue)
            {
                return _abpMailKitConfiguration.SecureSocketOption.Value;
            }

            return _abpMailKitConfiguration.EnableSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;
        }
    }
}