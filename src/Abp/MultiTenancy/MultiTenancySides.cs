using System;

namespace Abp.MultiTenancy
{
    /// <summary>
    /// Represents sides in a multi tenancy application.
    /// </summary>
    [Flags]
    public enum MultiTenancySides
    {
        /// <summary>
        /// Tenant side.
        /// </summary>
        Tenant = 1,
        
        /// <summary>
        /// Host (tenancy owner) side.
        /// </summary>
        Host = 2
    }
}