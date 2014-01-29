namespace Abp.Domain.Entities
{
    /// <summary>
    /// A shortcut of <see cref="AuditedEntity{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public abstract class AuditedEntity : AuditedEntity<int>, IEntity
    {

    }
}