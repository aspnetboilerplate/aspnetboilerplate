using System.Net.Mail;
using System.Threading.Tasks;

namespace Abp.Net.Mail
{
    /// <summary>
    /// This service can be used simply send emails over SMTP.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        Task SendAsync(string to, string subject, string body, bool isBodyHtml = true);

        /// <summary>
        /// Sends an email.
        /// </summary>
        Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true);

        /// <summary>
        /// Sends an email.
        /// </summary>
        Task SendAsync(MailMessage mail);
    }
}
