namespace Adorable.Net.Mail
{
    /// <summary>
    /// Declares names of the settings defined by <see cref="EmailSettingProvider"/>.
    /// </summary>
    public static class EmailSettingNames
    {
        /// <summary>
        /// Adorable.Net.Mail.DefaultFromAddress
        /// </summary>
        public const string DefaultFromAddress = "Adorable.Net.Mail.DefaultFromAddress";

        /// <summary>
        /// Adorable.Net.Mail.DefaultFromDisplayName
        /// </summary>
        public const string DefaultFromDisplayName = "Adorable.Net.Mail.DefaultFromDisplayName";

        /// <summary>
        /// SMTP related email settings.
        /// </summary>
        public static class Smtp
        {
            /// <summary>
            /// Adorable.Net.Mail.Smtp.Host
            /// </summary>
            public const string Host = "Adorable.Net.Mail.Smtp.Host";

            /// <summary>
            /// Adorable.Net.Mail.Smtp.Port
            /// </summary>
            public const string Port = "Adorable.Net.Mail.Smtp.Port";

            /// <summary>
            /// Adorable.Net.Mail.Smtp.UserName
            /// </summary>
            public const string UserName = "Adorable.Net.Mail.Smtp.UserName";

            /// <summary>
            /// Adorable.Net.Mail.Smtp.Password
            /// </summary>
            public const string Password = "Adorable.Net.Mail.Smtp.Password";

            /// <summary>
            /// Adorable.Net.Mail.Smtp.Domain
            /// </summary>
            public const string Domain = "Adorable.Net.Mail.Smtp.Domain";

            /// <summary>
            /// Adorable.Net.Mail.Smtp.EnableSsl
            /// </summary>
            public const string EnableSsl = "Adorable.Net.Mail.Smtp.EnableSsl";

            /// <summary>
            /// Adorable.Net.Mail.Smtp.UseDefaultCredentials
            /// </summary>
            public const string UseDefaultCredentials = "Adorable.Net.Mail.Smtp.UseDefaultCredentials";
        }
    }
}