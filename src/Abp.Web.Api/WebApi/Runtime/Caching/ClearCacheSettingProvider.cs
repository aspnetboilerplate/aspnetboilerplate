using System.Collections.Generic;
using Abp.Configuration;

namespace Abp.WebApi.Runtime.Caching
{
    public class ClearCacheSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(ClearCacheSettingSettingNames.Password, "123qweasdZXC")
            };
        }
    }
}
