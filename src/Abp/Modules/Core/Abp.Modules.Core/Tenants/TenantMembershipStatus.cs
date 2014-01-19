namespace Abp.Tenants
{
    /// <summary>
    /// Indicates membershipt status of a user for a tenant.
    /// </summary>
    public enum TenantMembershipStatus : byte
    {
        /// <summary>
        /// This user is waiting for tenant admin's approve for his membership.
        /// </summary>
        WaitingForApproval = 0,

        /// <summary>
        /// User is a member of the tenant.
        /// </summary>
        Member = 1
    }
}