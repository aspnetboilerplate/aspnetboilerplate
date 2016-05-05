using System;

namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// A shortcut of <see cref="FullAuditedEntity{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class FullAuditedEntity : FullAuditedEntity<Guid>
    {
    }
}