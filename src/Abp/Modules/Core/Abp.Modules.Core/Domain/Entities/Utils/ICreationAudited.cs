using System;

namespace Abp.Modules.Core.Domain.Entities.Utils
{
    /// <summary>
    /// This interface is implemented by entities which's creation informations (who and when created) must be stored.
    /// </summary>
    public interface ICreationAudited
    {
        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        User CreatorUser { get; set; }
    }
}