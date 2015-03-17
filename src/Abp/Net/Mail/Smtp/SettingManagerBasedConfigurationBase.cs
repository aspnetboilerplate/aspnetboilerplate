using Abp.Configuration;
using Abp.Extensions;

namespace Abp.Net.Mail.Smtp
{
    public abstract class SettingManagerBasedConfigurationBase
    {
        protected readonly ISettingManager SettingManager;

        protected SettingManagerBasedConfigurationBase(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        protected string GetNotEmptySettingValue(string name)
        {
            var value = SettingManager.GetSettingValue(name);
            if (value.IsNullOrEmpty())
            {
                throw new AbpException("No setting value defined for: " + name);
            }

            return value;
        }
    }
}