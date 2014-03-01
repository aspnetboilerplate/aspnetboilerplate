namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// A shortcut of <see cref="AuditedEntity{TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    public abstract class AuditedEntity : AuditedEntity<int>, IEntity
    {

    }
}