using Abp.Configuration;

namespace Abp.Net.Mail.Smtp
{
    public class SmtpClientConfiguration : MailConfigurationBase, ISmtpClientConfiguration
    {
        public string Host
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.Smtp.Host); }
        }

        public int Port
        {
            get { return SettingManager.GetSettingValue<int>(AbpEmailSettingNames.Smtp.Port); }
        }

        public string UserName
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.Smtp.UserName); }
        }

        public string Password
        {
            get { return GetNotEmptySettingValue(AbpEmailSettingNames.Smtp.Password); }
        }

        public string Domain
        {
            get { return SettingManager.GetSettingValue(AbpEmailSettingNames.Smtp.Domain); }
        }

        public bool EnableSsl
        {
            get { return SettingManager.GetSettingValue<bool>(AbpEmailSettingNames.Smtp.EnableSsl); }
        }

        public bool UseDefaultCredentials
        {
            get { return SettingManager.GetSettingValue<bool>(AbpEmailSettingNames.Smtp.UseDefaultCredentials); }
        }

        public SmtpClientConfiguration(ISettingManager settingManager)
            : base(settingManager)
        {
        }
    }
}