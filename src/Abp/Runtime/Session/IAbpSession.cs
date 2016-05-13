using Abp.MultiTenancy;

namespace Abp.Runtime.Session
{
    /// <summary>
    /// Defines some session information that can be useful for applications.
    /// </summary>
    public interface IAbpSession
    {
        /// <summary>
        /// Gets current UserId or null.
        /// It can be null if no user logged in.
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// Gets current TenantId or null.
        /// This TenantId should be the TenantId of the <see cref="UserId"/>.
        /// It can be null if given <see cref="UserId"/> is a host user or no user logged in.
        /// </summary>
        int? TenantId { get; }

        /// <summary>
        /// Gets current multi-tenancy side.
        /// </summary>
        MultiTenancySides MultiTenancySide { get; }

        /// <summary>
        /// UserId of the impersonator.
        /// This is filled if a user is performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        long? ImpersonatorUserId { get; }

        /// <summary>
        /// TenantId of the impersonator.
        /// This is filled if a user with <see cref="ImpersonatorUserId"/> performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        int? ImpersonatorTenantId { get; }
    }
}
