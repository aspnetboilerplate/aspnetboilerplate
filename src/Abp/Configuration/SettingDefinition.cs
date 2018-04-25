using System;
using Abp.Authorization;
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
        /// Can clients see this setting and it's value.
        /// It maybe dangerous for some settings to be visible to clients (such as email server password).
        /// Default: false.
        /// </summary>
        public bool IsVisibleToClients { get; set; }

        /// <summary>
        /// A permission dependency. Only users that can satisfy this permission dependency can see this setting on the client.
        /// Optional.
        /// </summary>
        public IPermissionDependency ClientVisibility { get; set; }

        /// <summary>
        /// This can be set to true if only authenticated users should see this setting on the client.
        /// </summary>
        public bool RequiresAuthentication { get; set; }

        /// <summary>
        /// Can be used to store a custom object related to this setting.
        /// </summary>
        public object CustomData { get; set; }

        /// <summary>
        /// Creates a new <see cref="SettingDefinition"/> object.
        /// </summary>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="defaultValue">Default value of the setting</param>
        /// <param name="displayName">Display name of the permission</param>
        /// <param name="group">Group of this setting</param>
        /// <param name="description">A brief description for this setting</param>
        /// <param name="scopes">Scopes of this setting. Default value: <see cref="SettingScopes.Application"/>.</param>
        /// <param name="isVisibleToClients">Can clients see this setting and it's value. Default: false</param>
        /// <param name="isInherited">Is this setting inherited from parent scopes. Default: True.</param>
        /// <param name="customData">Can be used to store a custom object related to this setting</param>
        /// <param name="requiresAuthentication">Requires authentication for setting's client visibility</param>
        /// <param name="clientVisibility">Requires permissions for setting's client visibility</param>
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
            bool requiresAuthentication = false,
            IPermissionDependency clientVisibility = null)
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
            IsVisibleToClients = isVisibleToClients;
            IsInherited = isInherited;
            CustomData = customData;
            RequiresAuthentication = requiresAuthentication;
            ClientVisibility = clientVisibility;
        }
    }
}
