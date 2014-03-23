using Abp.Localization;

namespace Abp.Configuration
{
    /// <summary>
    /// Defines a setting.
    /// A setting is used to configure and change behaviour of the application.
    /// </summary>
    public class SettingDefinition
    {
        /// <summary>
        /// Unique name of the setting.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Display name of the setting.
        /// This can be used to show setting to the user.
        /// </summary>
        public LocalizableString DisplayName { get; private set; }

        /// <summary>
        /// A brief description for this setting.
        /// </summary>
        public LocalizableString Description { get; private set; }

        /// <summary>
        /// Scopes of this setting.
        /// </summary>
        public SettingScopes Scopes { get; private set; }

        /// <summary>
        /// Gets/sets group for this setting.
        /// </summary>
        public SettingDefinitionGroup Group { get; private set; }

        /// <summary>
        /// Default value of the setting.
        /// </summary>
        public string DefaultValue { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SettingDefinition"/> object.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="defaultValue">Default value of the setting</param>
        /// <param name="displayName">Display name of the permission</param>
        /// <param name="group">Group of this setting</param>
        /// <param name="description">A brief description for this setting</param>
        /// <param name="scopes">Scopes of this setting</param>
        public SettingDefinition(string name, string defaultValue, LocalizableString displayName = null, SettingDefinitionGroup group = null, LocalizableString description = null, SettingScopes scopes = SettingScopes.Application)
        {
            Name = name;
            DefaultValue = defaultValue;
            DisplayName = displayName;
            Group = @group;
            Description = description;
            Scopes = scopes;
        }
    }
}
