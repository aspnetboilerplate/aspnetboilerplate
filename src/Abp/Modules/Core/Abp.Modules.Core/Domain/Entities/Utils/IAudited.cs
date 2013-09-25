namespace Abp.Modules.Core.Domain.Entities.Utils
{
    /// <summary>
    /// This interface is implemented by entities which must be audited.
    /// </summary>
    public interface IAudited : ICreationAudited, IModificationAudited
    {
        
    }
}