using Abp.Net.Mail.Smtp;
using MailKit.Security;

namespace Abp.MailKit
{

    /// <summary>
    /// Extends configurations to used by Mailkit-based SmtpClient object.
    /// </summary>
    public interface IAbpMailKitConfiguration : ISmtpEmailSenderConfiguration {

        /// <summary>
        /// If set, force the use of a specific socket option, else it is automatic
        /// </summary>
        SecureSocketOptions? SecureSocketOption { get; }

        /// <summary>
        ///     If set to true, force MailKit to accept certificates self-signed, expired, etc.
        ///     Default behaviour is: leave Mailkit to verify certificates
        /// </summary>
        bool? DisableCertificateValidation { get; }

        /// <summary>
        ///     If set to true, connections via SSL/TLS checks certificate revocation.
        ///     Normally, the value of this property should be set to true (the default) for security reasons
        ///     Default behaviour: if null, use the Mailkit default (check is active)
        /// </summary>
        bool? CheckCertificateRevocation { get; }
    }
}