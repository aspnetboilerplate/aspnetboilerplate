namespace Abp.Runtime.Session
{
    /// <summary>
    /// Defines some session informations that can be useful for applications.
    /// </summary>
    public interface IAbpSession
    {
        /// <summary>
        /// Gets current UserId of null.
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// Gets current TenantId or null.
        /// </summary>
        int? TenantId { get; }
    }
}
