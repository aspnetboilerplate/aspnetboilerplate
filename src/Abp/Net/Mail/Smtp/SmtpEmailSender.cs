using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Abp.Extensions;

namespace Abp.Net.Mail.Smtp
{
    /// <summary>
    /// Creates <see cref="SmtpClient"/> objects and configures it using  given configuration.
    /// </summary>
    public class SmtpEmailSender : ISmtpEmailSender
    {
        private readonly ISmtpEmailSenderConfiguration _configuration;

        /// <summary>
        /// Creates a new <see cref="SmtpEmailSender"/>.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public SmtpEmailSender(ISmtpEmailSenderConfiguration configuration)
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

        public async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(_configuration.DefaultFromAddress, to, subject, body, isBodyHtml);
        }

        public void Send(string to, string subject, string body, bool isBodyHtml = true)
        {
            Send(_configuration.DefaultFromAddress, to, subject, body, isBodyHtml);
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendAsync(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
        }

        public void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            Send(new MailMessage(from, to, subject, body) { IsBodyHtml = isBodyHtml });
        }

        public async Task SendAsync(MailMessage mail, bool normalize = true)
        {
            NormalizeMail(mail);
            using (var smtpClient = BuildClient())
            {
                await smtpClient.SendMailAsync(mail);
            }
        }

        public void Send(MailMessage mail, bool normalize = true)
        {
            NormalizeMail(mail);
            using (var smtpClient = BuildClient())
            {
                smtpClient.Send(mail);
            }
        }

        private void NormalizeMail(MailMessage mail)
        {
            if (mail.From == null || mail.From.Address.IsNullOrEmpty())
            {
                mail.From = new MailAddress(
                    _configuration.DefaultFromAddress,
                    _configuration.DefaultFromDisplayName,
                    Encoding.UTF8
                    );
            }

            if (mail.HeadersEncoding == null)
            {
                mail.HeadersEncoding = Encoding.UTF8;
            }

            if (mail.SubjectEncoding == null)
            {
                mail.SubjectEncoding = Encoding.UTF8;
            }

            if (mail.BodyEncoding == null)
            {
                mail.BodyEncoding = Encoding.UTF8;
            }
        }
    }
}