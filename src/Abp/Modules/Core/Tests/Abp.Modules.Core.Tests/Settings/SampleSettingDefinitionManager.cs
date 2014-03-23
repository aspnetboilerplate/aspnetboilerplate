using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Configuration;

namespace Abp.Modules.Core.Tests.Settings
{
    public class SampleSettingDefinitionManager : ISettingDefinitionManager
    {
        private readonly Dictionary<string, SettingDefinition> _settings;

        public SampleSettingDefinitionManager()
        {
            _settings = new Dictionary<string, SettingDefinition>
                        {
                            {"Language", new SettingDefinition("Language", "en")},
                            {"SiteTitle", new SettingDefinition("SiteTitle", "My Test Site", scopes: SettingScopes.Application)}
                        };
        }

        public SettingDefinition GetSettingDefinition(string name)
        {
            return _settings[name];
        }

        public IReadOnlyList<SettingDefinition> GetAllSettingDefinitions()
        {
            return _settings.Values.ToImmutableList();
        }
    }
}