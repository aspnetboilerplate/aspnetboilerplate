using System;

namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// A shortcut of <see cref="CreationAuditedEntity{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class CreationAuditedEntity : CreationAuditedEntity<Guid>
    {
    }
}