using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Abp.Extensions;

namespace Abp.Net.Mail.Smtp
{
    /// <summary>
    /// Creates <see cref="SmtpClient"/> objects and configures it using  given configuration.
    /// </summary>
    public class SmtpClientProvider : ISmtpClientProvider
    {
        private readonly ISmtpClientConfiguration _configuration;

        /// <summary>
        /// Creates a new <see cref="SmtpClientProvider"/>.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public SmtpClientProvider(ISmtpClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SmtpClient BuildClient()
        {
            var host = _configuration.Host;
            var port = _configuration.Port;

            var smtpClient = new SmtpClient(host, port);
            try
            {
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
            catch
            {
                smtpClient.Dispose();
                throw;
            }
        }

        public async Task SendEmailAsync(MailMessage mail)
        {
            using (var smtpClient = BuildClient())
            {
                await smtpClient.SendMailAsync(mail);
            }
        }

        public void SendEmail(MailMessage mail)
        {
            using (var smtpClient = BuildClient())
            {
                smtpClient.Send(mail);
            }
        }
    }
}