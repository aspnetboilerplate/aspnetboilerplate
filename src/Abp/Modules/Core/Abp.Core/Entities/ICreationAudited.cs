using System;
using Abp.Modules.Core.Entities.Core;

namespace Abp.Modules.Core.Entities
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