using Abp.Configuration;

namespace Abp.Net.Mail.Smtp
{
    public class SmtpClientProviderConfiguration : SettingManagerBasedConfigurationBase, ISmtpClientProviderConfiguration
    {
        public string Host
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.Host); }
        }

        public int Port
        {
            get { return SettingManager.GetSettingValue<int>(AbpEmailSettingNames.Port); }
        }

        public string UserName
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.UserName); }
        }

        public string Password
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.Password); }
        }

        public string Domain
        {
            get { return SettingManager.GetSettingValue(AbpEmailSettingNames.Domain); }
        }

        public bool EnableSsl
        {
            get { return SettingManager.GetSettingValue<bool>(AbpEmailSettingNames.EnableSsl); }
        }

        public bool UseDefaultCredentials
        {
            get { return SettingManager.GetSettingValue<bool>(AbpEmailSettingNames.UseDefaultCredentials); }
        }

        public SmtpClientProviderConfiguration(ISettingManager settingManager)
            : base(settingManager)
        {
        }
    }
}