using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Entities.Utils;

namespace Taskever.Entities
{
    /// <summary>
    /// Represents a task.
    /// </summary>
    public class Task : AuditedEntity, IHasTenant
    {
        /// <summary>
        /// The tenant account which this entity is belong to.
        /// </summary>
        public virtual Tenant Tenant { get; set; }

        /// <summary>
        /// Task title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Task description.
        /// </summary>
        public virtual string Description { get; set; }
    }
}
