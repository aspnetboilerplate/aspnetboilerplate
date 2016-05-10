using System;

namespace Abp
{
    /// <summary>
    ///     Interface to get a user identifier.
    /// </summary>
    public interface IUserIdentifier
    {
        /// <summary>
        ///     Tenant Id. Can be null for host users.
        /// </summary>
        Guid? TenantId { get; }

        /// <summary>
        ///     Id of the user.
        /// </summary>
        Guid UserId { get; }
    }
}