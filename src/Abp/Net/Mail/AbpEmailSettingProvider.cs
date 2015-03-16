using System.Collections.Generic;
using Abp.Configuration;

namespace Abp.Net.Mail
{
    //TODO: Register to SettingProviders on startup
    public class AbpEmailSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                   {
                       new SettingDefinition(AbpEmailSettingNames.Host, ""),
                       new SettingDefinition(AbpEmailSettingNames.Port, "25"),
                       new SettingDefinition(AbpEmailSettingNames.UserName, ""),
                       new SettingDefinition(AbpEmailSettingNames.Password, ""),
                       new SettingDefinition(AbpEmailSettingNames.Domain, "")
                   };
        }
    }
}