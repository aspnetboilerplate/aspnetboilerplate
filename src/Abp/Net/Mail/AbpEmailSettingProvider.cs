using System.Collections.Generic;
using Abp.Configuration;

namespace Abp.Net.Mail
{
    /// <summary>
    /// Defines settings to send emails.
    /// <see cref="AbpEmailSettingNames"/> for all available configurations.
    /// </summary>
    internal class AbpEmailSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                   {
                       new SettingDefinition(AbpEmailSettingNames.Smtp.Host, "127.0.0.1"),
                       new SettingDefinition(AbpEmailSettingNames.Smtp.Port, "25"),
                       new SettingDefinition(AbpEmailSettingNames.Smtp.UserName, ""),
                       new SettingDefinition(AbpEmailSettingNames.Smtp.Password, ""),
                       new SettingDefinition(AbpEmailSettingNames.Smtp.Domain, ""),
                       new SettingDefinition(AbpEmailSettingNames.Smtp.EnableSsl, "false"),
                       new SettingDefinition(AbpEmailSettingNames.Smtp.UseDefaultCredentials, "true"),
                       new SettingDefinition(AbpEmailSettingNames.DefaultFromAddress, ""),
                       new SettingDefinition(AbpEmailSettingNames.DefaultFromDisplayName, "")
                   };
        }
    }
}