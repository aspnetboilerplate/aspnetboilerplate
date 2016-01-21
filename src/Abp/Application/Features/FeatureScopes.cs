using System;

namespace Abp.Application.Features
{
    /// <summary>
    /// Scopes of a <see cref="Feature"/>.
    /// </summary>
    [Flags]
    public enum FeatureScopes
    {
        /// <summary>
        /// This <see cref="Feature"/> can be enabled/disabled per edition.
        /// </summary>
        Edition = 1,

        /// <summary>
        /// This Feature<see cref="Feature"/> can be enabled/disabled per tenant.
        /// </summary>
        Tenant = 2,

        /// <summary>
        /// This <see cref="Feature"/> can be enabled/disabled per tenant and edition.
        /// </summary>
        All = 3
    }
}