using System.Net.Mail;

namespace Abp.Net.Mail.Smtp
{
    /// <summary>
    /// Used to create <see cref="SmtpClient"/> objects that is configured and ready to use.
    /// </summary>
    public interface ISmtpClientProvider
    {
        /// <summary>
        /// Creates and configures new <see cref="SmtpClient"/> object to send emails. 
        /// </summary>
        /// <returns>
        /// An <see cref="SmtpClient"/> object that is ready to send emails.
        /// </returns>
        SmtpClient CreateSmtpClient();
    }
}