namespace Abp.MailKit
{
    /// <summary>
    /// Declares names of the settings defined by <see cref="MailKitEmailSettingProvider"/>.
    /// </summary>
    public static class MailKitEmailSettingNames
    {
        /// <summary>
        /// SMTP related email settings.
        /// </summary>
        public static class Smtp
        {
            /// <summary>
            /// AbAbp.Net.Mail.Smtp.MailKit.SecureSocketOption
            /// </summary>
            public const string SecureSocketOption = "Abp.Net.Mail.Smtp.MailKit.SecureSocketOption";

            /// <summary>
            /// AbAbp.Net.Mail.Smtp.MailKit.CheckCertificateRevocation
            /// </summary>
            public const string CheckCertificateRevocation = "Abp.Net.Mail.Smtp.MailKit.CheckCertificateRevocation";

            /// <summary>
            /// Abp.Net.Mail.Smtp.MailKit.DisableCertificateValidation
            /// </summary>
            public const string DisableCertificateValidation = "Abp.Net.Mail.Smtp.MailKit.DisableCertificateValidation";

        }
    }
}