namespace Abp.Domain.Entities
{
    /// <summary>
    /// A shortcut of <see cref="ModificationAuditedEntity{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public abstract class ModificationAuditedEntity : ModificationAuditedEntity<int>
    {

    }
}