using System;

namespace Abp.Application.Features
{
    /// <summary>
    /// Scopes of a <see cref="ApplicationFeature"/>.
    /// </summary>
    [Flags]
    public enum ApplicationFeatureScopes
    {
        /// <summary>
        /// This Feature can be enabled/disabled per edition.
        /// </summary>
        Edition = 1,

        /// <summary>
        /// This Feature can be enabled/disabled per tenant.
        /// </summary>
        Tenant = 2,

        /// <summary>
        /// This Feature can be enabled/disabled per tenant and edition.
        /// </summary>
        All = 3
    }
}