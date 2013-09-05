namespace Abp.Entities
{
    /// <summary>
    /// This interface is implemented by entities which must be audited.
    /// </summary>
    public interface IAudited : ICreationAudited, IModificationAudited
    {
        
    }
}