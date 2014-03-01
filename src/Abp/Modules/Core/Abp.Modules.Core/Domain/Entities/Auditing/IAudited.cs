namespace Abp.Domain.Entities.Auditing
{
    /// <summary>
    /// This interface is implemented by entities which must be audited.
    /// Related properties automatically set when saving/updating <see cref="IEntity"/>.
    /// </summary>
    public interface IAudited : ICreationAudited, IModificationAudited
    {
        
    }
}