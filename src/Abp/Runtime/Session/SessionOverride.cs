namespace Abp.Runtime.Session
{
    public class SessionOverride
    {
        public long? UserId { get; }
        public long? BranchId { get; }
        public long? TenantId { get; }

        public SessionOverride(long? tenantId, long? userId, long? branchId)
        {
            TenantId = tenantId;
            UserId = userId;
            BranchId = branchId;
        }
    }
}