using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Configuration
{
    /// <summary>
    /// Implements <see cref="ISettingDefinitionManager"/>.
    /// </summary>
    internal class SettingDefinitionManager : ISettingDefinitionManager, ISingletonDependency
    {
        private readonly IDictionary<string, SettingDefinition> _settings;

        public IAssemblyFinder AssemblyFinder { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingDefinitionManager()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
            _settings = new Dictionary<string, SettingDefinition>();
            LoadAllSettingsFromAllProviders();
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

        private void LoadAllSettingsFromAllProviders()
        {
            var context = new SettingDefinitionProviderContext();
            foreach (var assembly in AssemblyFinder.GetAllAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(ISettingDefinitionProvider).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        var provider = (ISettingDefinitionProvider)Activator.CreateInstance(type);
                        foreach (var settings in provider.GetSettingDefinitions(context))
                        {
                            _settings[settings.Name] = settings;
                        }
                    }
                }
            }
        }
    }
}