using Abp.Modules.Core.Domain.Entities.Utils;

namespace Abp.Modules.Core.Domain.Entities
{
    /// <summary>
    /// Represents a role in an application.
    /// </summary>
    public class Role : AuditedEntity, IHasTenant
    {
        /// <summary>
        /// Tenant of this role.
        /// </summary>
        public virtual Tenant Tenant { get; set; }

        /// <summary>
        /// Unique name of this role.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Display name of this role.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Is this role static (can not be deleted).
        /// Static roles can be used programmatically.
        /// </summary>
        public virtual bool IsStatic { get; set; }

        /// <summary>
        /// Is this role frozen (can not edit it's features).
        /// Only static (see <see cref="IsStatic"/>) roles can be frozen!
        /// </summary>
        public virtual bool IsFrozen { get; set; }
    }
}
