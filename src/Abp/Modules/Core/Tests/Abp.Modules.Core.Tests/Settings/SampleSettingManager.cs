using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Configuration;

namespace Abp.Modules.Core.Tests.Settings
{
    public class SampleSettingManager : ISettingManager
    {
        private readonly Dictionary<string, Setting> _settings;

        public SampleSettingManager()
        {
            _settings = new Dictionary<string, Setting>
                        {
                            {"Language", new Setting("Language", "en")},
                            {"SiteTitle", new Setting("SiteTitle", "My Test Site", scopes: SettingScopes.Application)}
                        };
        }

        public Setting GetSetting(string name)
        {
            return _settings[name];
        }

        public IReadOnlyList<Setting> GetAllSettings()
        {
            return _settings.Values.ToImmutableList();
        }
    }
}