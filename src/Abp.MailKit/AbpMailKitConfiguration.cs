using System;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Net.Mail.Smtp;
using MailKit.Security;

namespace Abp.MailKit
{
    public class AbpMailKitConfiguration : SmtpEmailSenderConfiguration, IAbpMailKitConfiguration
    {
        public SecureSocketOptions? SecureSocketOption {
            get {
                var valueAsString = SettingManager.GetSettingValue(MailKitEmailSettingNames.Smtp.SecureSocketOption);
                if (string.IsNullOrWhiteSpace(valueAsString)) return null;

                SecureSocketOptions result;
                return Enum.TryParse(valueAsString, true, out result) ? result : SecureSocketOptions.Auto;
            }
        }

        public bool? DisableCertificateValidation {
            get {
                var valueAsString = SettingManager.GetSettingValue(MailKitEmailSettingNames.Smtp.DisableCertificateValidation);
                return !string.IsNullOrWhiteSpace(valueAsString) ? valueAsString.To<bool>() : (bool?)null;
            }
        }

        public bool? CheckCertificateRevocation {
            get {
                var valueAsString = SettingManager.GetSettingValue(MailKitEmailSettingNames.Smtp.CheckCertificateRevocation);
                return !string.IsNullOrWhiteSpace(valueAsString) ? valueAsString.To<bool>() : (bool?)null;
            }
        }

        /// <summary>
        /// Creates a new <see cref="AbpMailKitConfiguration"/>.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        public AbpMailKitConfiguration(ISettingManager settingManager)
            : base(settingManager) {

        }
    }
}
