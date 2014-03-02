using System.Collections.Generic;
using Abp.Configuration;

namespace Abp.Net.Mail
{
    /// <summary>
    /// Provide settings for email.
    /// </summary>
    public class EmailSettingProvider : ISettingProvider
    {
        public IEnumerable<Setting> GetSettings(SettingManager settingManager)
        {
            return new[]
                   {
                       new Setting("Abp.Net.Mail.SenderAddress", ""),
                       new Setting("Abp.Net.Mail.DisplayName", ""),
                       new Setting("Abp.Net.Mail.ServerAddress", ""),
                       new Setting("Abp.Net.Mail.ServerPort", ""),
                       new Setting("Abp.Net.Mail.Username", ""),
                       new Setting("Abp.Net.Mail.Password", "")
                   };
        }
    }
}
