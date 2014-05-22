using System.Collections.Generic;
using Abp.Configuration;
using Abp.Localization;

namespace Taskever.Utils.Mail
{
    /// <summary>
    /// Provide settings for email.
    /// </summary>
    public class EmailSettingDefinitionProvider : ISettingDefinitionProvider
    {
        public IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                   {
                       new SettingDefinition("Abp.Net.Mail.Username", "", new LocalizableString("UserName","Taskever")),
                       new SettingDefinition("Abp.Net.Mail.Password", "", new LocalizableString("Password","Taskever")),
                       new SettingDefinition("Abp.Net.Mail.ServerAddress", "", new LocalizableString("ServerAddress","Taskever"), isVisibleToClients: true),
                       new SettingDefinition("Abp.Net.Mail.ServerPort", "", new LocalizableString("ServerPort","Taskever")),
                       new SettingDefinition("Abp.Net.Mail.DisplayName", "", new LocalizableString("DisplayName","Taskever")),
                       new SettingDefinition("Abp.Net.Mail.SenderAddress", "", new LocalizableString("SenderAddress","Taskever"))
                   };
        }
    }
}
