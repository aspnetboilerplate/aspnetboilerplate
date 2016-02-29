using System.Collections.Generic;
using Adorable.Configuration;

namespace Adorable.WebApi.Runtime.Caching
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
