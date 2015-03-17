using Abp.Configuration;

namespace Abp.Net.Mail
{
    /// <summary>
    /// Implementation of <see cref="IEmailSenderConfiguration"/> that reads settings
    /// from <see cref="ISettingManager"/>.
    /// </summary>
    public class EmailSenderConfiguration : MailConfigurationBase, IEmailSenderConfiguration
    {
        public string DefaultFromAddress
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.DefaultFromAddress); }
        }

        public string DefaultFromDisplayName
        {
            get { return SettingManager.GetSettingValue(AbpEmailSettingNames.DefaultFromDisplayName); }
        }
        
        /// <summary>
        /// Creates a new <see cref="EmailSenderConfiguration"/>.
        /// </summary>
        public EmailSenderConfiguration(ISettingManager settingManager)
            : base(settingManager)
        {
        }
    }
}