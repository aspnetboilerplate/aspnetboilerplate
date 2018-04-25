using Abp.Authorization;

namespace Abp.Configuration
{
    public class SettingDefinitionClientVisibility
    {
        /// <summary>
        /// Can clients see this setting and it's value.
        /// It maybe dangerous for some settings to be visible to clients (such as email server password).
        /// Default: false.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// This can be set to true if only authenticated users should see this setting on the client.
        /// </summary>
        public bool RequiresAuthentication { get; set; }

        /// <summary>
        /// A permission dependency. Only users that can satisfy this permission dependency can see this setting on the client.
        /// Optional.
        /// </summary>
        public IPermissionDependency PermissionDependency { get; set; }

        public SettingDefinitionClientVisibility(
            bool isVisible = false, 
            bool requiresAuthentication = false, 
            IPermissionDependency permissionDependency = null
        )
        {
            IsVisible = isVisible;
            RequiresAuthentication = requiresAuthentication;
            PermissionDependency = permissionDependency;
        }
    }
}