namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// This interface is implemented by entities which's creation informations (who and when created) must be stored.
    /// Creation time and creator user are automatically set when saving <see cref="Entity"/> to database.
    /// </summary>
    public interface ICreationAudited : IHasCreationTime, IHasCreatorUser
    {

    }
}