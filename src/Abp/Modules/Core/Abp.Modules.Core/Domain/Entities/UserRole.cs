using Abp.Modules.Core.Domain.Entities.Utils;

namespace Abp.Modules.Core.Domain.Entities
{
    /// <summary>
    /// Represents role record of a user.
    /// </summary>
    public class UserRole : CreationAuditedEntity, IHasTenant
    {
        /// <summary>
        /// Tenant.
        /// </summary>
        public virtual Tenant Tenant { get; set; }

        /// <summary>
        /// User.
        /// </summary>
        public virtual Tenant User { get; set; }

        /// <summary>
        /// Role.
        /// </summary>
        public virtual Role Role { get; set; }
    }
}