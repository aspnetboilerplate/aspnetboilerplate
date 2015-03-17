using System.Net;
using System.Net.Mail;
using Abp.Extensions;

namespace Abp.Net.Mail.Smtp
{
    /// <summary>
    /// Creates <see cref="SmtpClient"/> objects and configures it using  given configuration.
    /// </summary>
    public class SmtpClientProvider : ISmtpClientProvider
    {
        private readonly ISmtpClientProviderConfiguration _configuration;

        public SmtpClientProvider(ISmtpClientProviderConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SmtpClient CreateSmtpClient()
        {
            var host = _configuration.Host;
            var port = _configuration.Port;

            var smtpClient = new SmtpClient(host, port);

            if (_configuration.EnableSsl)
            {
                smtpClient.EnableSsl = true;
            }

            if (_configuration.UseDefaultCredentials)
            {
                smtpClient.UseDefaultCredentials = true;
            }
            else
            {
                smtpClient.UseDefaultCredentials = false;

                var userName = _configuration.UserName;
                var password = _configuration.Password;
                var domain = _configuration.Domain;

                smtpClient.Credentials = !domain.IsNullOrEmpty()
                    ? new NetworkCredential(userName, password, domain)
                    : new NetworkCredential(userName, password);
            }

            return smtpClient;
        }
    }
}