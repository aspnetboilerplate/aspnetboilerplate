using System.Threading.Tasks;

namespace Abp.Organizations
{
    /// <summary>
    /// Used to get settings related to OrganizationUnits.
    /// </summary>
    public interface IOrganizationUnitSettings
    {
        /// <summary>
        /// GetsMaximum allowed organization unit membership count for a user.
        /// Returns value for current tenant.
        /// </summary>
        int MaxUserMembershipCount { get; }

        /// <summary>
        /// Gets Maximum allowed organization unit membership count for a user.
        /// Returns value for given tenant.
        /// </summary>
        /// <param name="tenantId">The tenant Id or null for the host.</param>
        Task<int> GetMaxUserMembershipCountAsync(int? tenantId);

        /// <summary>
        /// Sets Maximum allowed organization unit membership count for a user.
        /// </summary>
        /// <param name="tenantId">The tenant Id or null for the host.</param>
        /// <param name="value">Setting value.</param>
        /// <returns></returns>
        Task SetMaxUserMembershipCountAsync(int? tenantId, int value);
    }
}