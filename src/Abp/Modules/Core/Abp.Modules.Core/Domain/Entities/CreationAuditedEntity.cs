namespace Abp.Domain.Entities
{
    /// <summary>
    /// A shortcut of <see cref="CreationAuditedEntity{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public abstract class CreationAuditedEntity : CreationAuditedEntity<int>
    {

    }
}