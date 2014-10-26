using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Configuration
{
    /// <summary>
    /// Implements <see cref="ISettingDefinitionManager"/>.
    /// </summary>
    internal class SettingDefinitionManager : ISettingDefinitionManager, ISingletonDependency
    {
        private readonly IIocManager _iocManager;
        private readonly ISettingsConfiguration _settingsConfiguration;
        private readonly IDictionary<string, SettingDefinition> _settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingDefinitionManager(IIocManager iocManager, ISettingsConfiguration settingsConfiguration)
        {
            _iocManager = iocManager;
            _settingsConfiguration = settingsConfiguration;
            _settings = new Dictionary<string, SettingDefinition>();
        }

        public void Initialize()
        {
            var context = new SettingDefinitionProviderContext();

            foreach (var providerType in _settingsConfiguration.Providers)
            {
                var provider = CreateProvider(providerType);
                foreach (var settings in provider.GetSettingDefinitions(context))
                {
                    _settings[settings.Name] = settings;
                }
            }
        }

        public SettingDefinition GetSettingDefinition(string name)
        {
            SettingDefinition settingDefinition;
            if (!_settings.TryGetValue(name, out settingDefinition))
            {
                throw new AbpException("There is no setting defined with name: " + name);
            }

            return settingDefinition;
        }

        public IReadOnlyList<SettingDefinition> GetAllSettingDefinitions()
        {
            return _settings.Values.ToImmutableList();
        }

        private SettingProvider CreateProvider(Type providerType)
        {
            if (!_iocManager.IsRegistered(providerType))
            {
                _iocManager.Register(providerType);
            }

            return (SettingProvider)_iocManager.Resolve(providerType);
        }
    }
}