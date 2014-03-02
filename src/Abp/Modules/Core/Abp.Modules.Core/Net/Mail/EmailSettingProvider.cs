using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;

namespace Abp.Net.Mail
{
    /// <summary>
    /// Provide settings for email.
    /// </summary>
    public class EmailSettingProvider : ISettingProvider
    {
        public IEnumerable<Setting> GetSettings()
        {
            return new[]
                   {
                       new Setting("Abp.Net.Mail.Username", "", new LocalizableString("UserName","Abp.Modules.Core")),
                       new Setting("Abp.Net.Mail.Password", "", new LocalizableString("Password","Abp.Modules.Core")),
                       new Setting("Abp.Net.Mail.ServerAddress", "", new LocalizableString("ServerAddress","Abp.Modules.Core")),
                       new Setting("Abp.Net.Mail.ServerPort", "", new LocalizableString("ServerPort","Abp.Modules.Core")),
                       new Setting("Abp.Net.Mail.DisplayName", "", new LocalizableString("DisplayName","Abp.Modules.Core")),
                       new Setting("Abp.Net.Mail.SenderAddress", "", new LocalizableString("SenderAddress","Abp.Modules.Core"))
                   };
        }
    }
}
