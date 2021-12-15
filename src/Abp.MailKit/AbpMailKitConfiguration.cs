using System;
using Abp.Configuration;
using Abp.Net.Mail.Smtp;
using MailKit.Security;

namespace Abp.MailKit
{
    public class AbpMailKitConfiguration : SmtpEmailSenderConfiguration, IAbpMailKitConfiguration
    {
        public SecureSocketOptions? SecureSocketOption
        { 
            get
            {
                var valueAsString = SettingManager.GetSettingValue(MailKitEmailSettingNames.Smtp.SecureSocketOption);
                if (string.IsNullOrWhiteSpace(valueAsString)) return null;

                SecureSocketOptions result;
                return Enum.TryParse(valueAsString, true, out result) ? result : SecureSocketOptions.Auto;
            } 
        }

        public bool? DisableCertificateValidation => SettingManager.GetSettingValue<bool>(MailKitEmailSettingNames.Smtp.DisableCertificateValidation);

        public bool? CheckCertificateRevocation => SettingManager.GetSettingValue<bool>(MailKitEmailSettingNames.Smtp.CheckCertificateRevocation);

        /// <summary>
        /// Creates a new <see cref="AbpMailKitConfiguration"/>.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        public AbpMailKitConfiguration(ISettingManager settingManager)
            : base(settingManager) {

        }
    }
}
