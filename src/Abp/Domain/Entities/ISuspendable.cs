namespace Abp.Domain.Entities
{
    /// <summary>
    /// This interface is used to make an entity suspendable.
    /// </summary>
    public interface ISuspendable
    {
        /// <summary>
        /// True: This entity is suspended for a while.
        /// False: This entity is not suspended.
        /// </summary>
        bool IsSuspended { get; set; }
    }
}