using Abp.Configuration;

namespace Abp.Net.Mail.Smtp
{
    public class SmtpEmailSenderConfiguration : SettingManagerBasedConfigurationBase, ISmtpEmailSenderConfiguration
    {
        public string DefaultFromAddress
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.DefaultFromAddress); }
        }

        public string DefaultFromDisplayName
        {
            get { return SettingManager.GetSettingValue(AbpEmailSettingNames.DefaultFromDisplayName); }
        }
        
        public SmtpEmailSenderConfiguration(ISettingManager settingManager)
            : base(settingManager)
        {
        }
    }
}