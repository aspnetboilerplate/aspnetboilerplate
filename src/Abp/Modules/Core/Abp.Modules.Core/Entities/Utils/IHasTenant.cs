namespace Abp.Modules.Core.Entities.Utils
{
    /// <summary>
    /// Implemented by entities those has a tenant.
    /// </summary>
    public interface IHasTenant
    {
        /// <summary>
        /// The tenant account which this entity is belong to.
        /// </summary>
        Tenant Tenant { get; set; }
    }
}