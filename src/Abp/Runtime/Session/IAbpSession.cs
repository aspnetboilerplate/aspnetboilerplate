using Abp.MultiTenancy;
using System;

namespace Abp.Runtime.Session
{
    /// <summary>
    /// Defines some session information that can be useful for applications.
    /// </summary>
    public interface IAbpSession
    {
        /// <summary>
        /// Gets current UserId or null.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Gets current TenantId or null.
        /// </summary>
        Guid? TenantId { get; }

        /// <summary>
        /// Gets current multi-tenancy side.
        /// </summary>
        MultiTenancySides MultiTenancySide { get; }

        /// <summary>
        /// UserId of the impersonator.
        /// This is filled if a user is performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        Guid? ImpersonatorUserId { get; }

        /// <summary>
        /// TenantId of the impersonator.
        /// This is filled if a user with <see cref="ImpersonatorUserId"/> performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        Guid? ImpersonatorTenantId { get; }
    }
}