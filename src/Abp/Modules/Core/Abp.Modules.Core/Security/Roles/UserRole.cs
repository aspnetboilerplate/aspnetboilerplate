using Abp.Domain.Entities;
using Abp.Security.Users;

namespace Abp.Security.Roles
{
    /// <summary>
    /// Represents role record of a user.
    /// TODO: Add a unique index for UserId, RoleId
    /// </summary>
    public class UserRole : CreationAuditedEntity<long>
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual AbpUser User { get; set; }

        /// <summary>
        /// Role.
        /// </summary>
        public virtual AbpRole Role { get; set; }
    }
}