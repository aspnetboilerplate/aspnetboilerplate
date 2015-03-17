using System;
using Abp.Configuration;
using Abp.Extensions;

namespace Abp.Net.Mail
{
    /// <summary>
    /// Base class for email configuration classes.
    /// </summary>
    public abstract class MailConfigurationBase
    {
        protected readonly ISettingManager SettingManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected MailConfigurationBase(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        /// <summary>
        /// Gets a setting value by checking. Throws <see cref="AbpException"/> if it's null or empty.
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <returns>Value of the setting</returns>
        protected string GetNotEmptySettingValue(string name)
        {
            var value = SettingManager.GetSettingValue(name);
            if (value.IsNullOrEmpty())
            {
                throw new AbpException(String.Format("Setting value for '{0}' is null or empty!", name));
            }

            return value;
        }
    }
}