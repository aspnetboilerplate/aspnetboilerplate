using Abp.Domain.Entities;
using Abp.Security.Users;

namespace Abp.Security.Roles
{
    /// <summary>
    /// Represents role record of a user.
    /// </summary>
    public class UserRole : CreationAuditedEntity
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Role.
        /// </summary>
        public virtual Role Role { get; set; }
    }
}