using System;

namespace Abp.MultiTenancy
{
    /// <summary>
    /// Represents sides in a multi tenancy application.
    /// </summary>
    [Flags]
    public enum MultiTenancySide
    {
        Tenant,
        
        Host
    }
}