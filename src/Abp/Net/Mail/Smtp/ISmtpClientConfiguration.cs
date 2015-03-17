using System.Net.Mail;

namespace Abp.Net.Mail.Smtp
{
    /// <summary>
    /// Defines configurations to used by <see cref="SmtpClient"/> object.
    /// </summary>
    public interface ISmtpClientConfiguration
    {
        string Host { get; }

        int Port { get; }

        string UserName { get; }

        string Password { get; }

        string Domain { get; }

        bool EnableSsl { get; }

        bool UseDefaultCredentials { get; }
    }
}