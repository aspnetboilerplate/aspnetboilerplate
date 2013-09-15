using Abp.Modules.Core.Entities.Utils;

namespace Abp.Modules.Core.Entities
{
    /// <summary>
    /// Represents membership of a user for a tenant.
    /// </summary>
    public class TenantUser : CreationAuditedEntity, IHasTenant
    {
        /// <summary>
        /// Tenant.
        /// </summary>
        public virtual Tenant Tenant { get; set; }
        
        /// <summary>
        /// User.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Membership status.
        /// </summary>
        public virtual TenantMembershipStatus MembershipStatus { get; set; }

        /// <summary>
        /// The user who approved this membership.
        /// </summary>
        public virtual User ApprovedUser { get; set; }
    }
}