namespace Abp.Domain.Entities
{
    /// <summary>
    /// This interface is used to make an entity active/passive.
    /// </summary>
    public interface IPassivable
    {
        /// <summary>
        /// True: This entity is active.
        /// False: This entity is not active.
        /// </summary>
        bool IsActive { get; set; }
    }
}