using System;
using Abp.Localization;

namespace Abp.Configuration
{
    /// <summary>
    /// Defines a setting.
    /// A setting is used to configure and change behavior of the application.
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
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// A brief description for this setting.
        /// </summary>
        public ILocalizableString Description { get; set; }

        /// <summary>
        /// Scopes of this setting.
        /// Default value: <see cref="SettingScopes.Application"/>.
        /// </summary>
        public SettingScopes Scopes { get; set; }

        /// <summary>
        /// Is this setting inherited from parent scopes.
        /// Default: True.
        /// </summary>
        public bool IsInherited { get; set; }

        /// <summary>
        /// Gets/sets group for this setting.
        /// </summary>
        public SettingDefinitionGroup Group { get; set; }

        /// <summary>
        /// Default value of the setting.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Client visibility definition for the setting.
        /// </summary>
        public ISettingClientVisibilityProvider ClientVisibilityProvider { get; set; }

        /// <summary>
        /// Can be used to store a custom object related to this setting.
        /// </summary>
        public object CustomData { get; set; }

        /// <summary>
        /// Is this setting stored as encrypted in the data source.
        /// Default: False.
        /// </summary>
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// Creates a new <see cref="SettingDefinition"/> object.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="defaultValue">Default value of the setting</param>
        /// <param name="displayName">Display name of the permission</param>
        /// <param name="group">Group of this setting</param>
        /// <param name="description">A brief description for this setting</param>
        /// <param name="scopes">Scopes of this setting. Default value: <see cref="SettingScopes.Application"/>.</param>
        /// <param name="isVisibleToClients">This parameter is obsolete. Use <paramref name="clientVisibilityProvider"/> instead! Default: false</param>
        /// <param name="isInherited">Is this setting inherited from parent scopes. Default: True.</param>
        /// <param name="customData">Can be used to store a custom object related to this setting</param>
        /// <param name="clientVisibilityProvider">Client visibility definition for the setting. Default: invisible</param>
        /// <param name="isEncrypted">Is this setting stored as encrypted in the data source.</param>
        public SettingDefinition(
            string name,
            string defaultValue,
            ILocalizableString displayName = null,
            SettingDefinitionGroup group = null,
            ILocalizableString description = null,
            SettingScopes scopes = SettingScopes.Application,
            bool isVisibleToClients = false,
            bool isInherited = true,
            object customData = null,
            ISettingClientVisibilityProvider clientVisibilityProvider = null,
            bool isEncrypted = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            DefaultValue = defaultValue;
            DisplayName = displayName;
            Group = @group;
            Description = description;
            Scopes = scopes;
            IsInherited = isInherited;
            CustomData = customData;
            IsEncrypted = isEncrypted;

            ClientVisibilityProvider = new HiddenSettingClientVisibilityProvider();

            if (isVisibleToClients)
            {
                ClientVisibilityProvider = new VisibleSettingClientVisibilityProvider();
            }
            else if (clientVisibilityProvider != null)
            {
                ClientVisibilityProvider = clientVisibilityProvider;
            }
        }
    }
}
