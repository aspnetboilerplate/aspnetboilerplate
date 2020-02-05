using System;
using Abp.Application.Features;
using Abp.Extensions;
using Abp.Localization;

namespace Abp.Webhooks
{
    public class WebhookDefinition
    {
        /// <summary>
        /// Unique name of the webhook.
        /// </summary>
        public string Name { get; }

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
        /// A feature dependency. This webhook will be available to a tenant if this feature is enabled.
        /// Optional.
        /// </summary>
        public IFeatureDependency FeatureDependency { get; set; }

        public WebhookDefinition(string name, ILocalizableString displayName = null, ILocalizableString description = null, IFeatureDependency featureDependency = null)
        {
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            Name = name.Trim();
            DisplayName = displayName;
            Description = description;
            FeatureDependency = featureDependency;
        }
    }
}
