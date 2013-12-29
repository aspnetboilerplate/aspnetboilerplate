namespace Abp.Domain.Entities
{
    /// <summary>
    /// A shortcut of <see cref="Entity{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public abstract class Entity : Entity<int>
    {

    }
}