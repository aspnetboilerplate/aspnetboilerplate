using System.Collections.Generic;
using Abp.Configuration;

namespace Abp.Net.Mail
{
    /// <summary>
    /// Defines settings to send emails.
    /// </summary>
    internal class AbpEmailSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                   {
                       new SettingDefinition(AbpEmailSettingNames.Host, "127.0.0.1"),
                       new SettingDefinition(AbpEmailSettingNames.Port, "25"),
                       new SettingDefinition(AbpEmailSettingNames.UserName, ""),
                       new SettingDefinition(AbpEmailSettingNames.Password, ""),
                       new SettingDefinition(AbpEmailSettingNames.Domain, ""),
                       new SettingDefinition(AbpEmailSettingNames.EnableSsl, "false"),
                       new SettingDefinition(AbpEmailSettingNames.UseDefaultCredentials, "true"),
                       new SettingDefinition(AbpEmailSettingNames.DefaultFromAddress, ""),
                       new SettingDefinition(AbpEmailSettingNames.DefaultFromDisplayName, "")
                   };
        }
    }
}