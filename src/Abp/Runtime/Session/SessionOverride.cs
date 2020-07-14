namespace Abp.Runtime.Session
{
    public class SessionOverride
    {
        public long? UserId { get; }
        public long? BranchId { get; }

        public int? TenantId { get; }

        public SessionOverride(int? tenantId, long? userId, long? branchId)
        {
            TenantId = tenantId;
            UserId = userId;
            BranchId = branchId;
        }
    }
}