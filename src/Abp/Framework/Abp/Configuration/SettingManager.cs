using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Exceptions;

namespace Abp.Configuration
{
    /// <summary>
    /// Implements <see cref="ISettingManager"/>.
    /// </summary>
    public class SettingManager : ISettingManager
    {
        private readonly IDictionary<string, Setting> _settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingManager()
        {
            _settings = new Dictionary<string, Setting>();
            LoadAllSettingsFromAllProviders();
        }

        public Setting GetSetting(string name)
        {
            Setting setting;
            if (!_settings.TryGetValue(name, out setting))
            {
                throw new AbpException("There is no setting defined with name: " + name);
            }

            return setting;
        }

        public IReadOnlyList<Setting> GetAllSettings()
        {
            return _settings.Values.ToImmutableList();
        }

        private void LoadAllSettingsFromAllProviders()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(ISettingProvider).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        var provider = (ISettingProvider)Activator.CreateInstance(type);
                        foreach (var settings in provider.GetSettings(this))
                        {
                            _settings[settings.Name] = settings;
                        }
                    }
                }
            }
        }
    }
}