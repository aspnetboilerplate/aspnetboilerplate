using System;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Localization;

namespace Abp.WebHooks
{
    public class WebHookDefinition
    {
        /// <summary>
        /// Unique name of the webhook.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Display name of the webhook.
        /// Optional.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// Description for the webhook.
        /// Optional.
        /// </summary>
        public ILocalizableString Description { get; set; }

        /// <summary>
        /// A permission dependency. This webhook will be available to a user if this dependency is satisfied.
        /// Optional.
        /// </summary>
        public IPermissionDependency PermissionDependency { get; set; }

        /// <summary>
        /// A feature dependency. This webhook will be available to a tenant if this feature is enabled.
        /// Optional.
        /// </summary>
        public IFeatureDependency FeatureDependency { get; set; }

        public WebHookDefinition(string name, ILocalizableString displayName = null, ILocalizableString description = null, IPermissionDependency permissionDependency = null, IFeatureDependency featureDependency = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            if (name.Contains(";"))
            {
                throw new ArgumentException($"{nameof(name)} can not contain ';'", nameof(name));
            }

            Name = name;
            DisplayName = displayName;
            Description = description;
            PermissionDependency = permissionDependency;
            FeatureDependency = featureDependency;
        }
    }
}
