using Abp.Configuration;
using System.Collections.Generic;

namespace Abp.WebApi.Runtime.Caching
{
    public class ClearCacheSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(ClearCacheSettingNames.Password, "123qweasdZXC")
            };
        }
    }
}