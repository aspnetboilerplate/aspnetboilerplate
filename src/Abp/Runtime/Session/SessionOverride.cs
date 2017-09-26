namespace Abp.Runtime.Session
{
    public class SessionOverride
    {
        public long? UserId { get; }

        public int? TenantId { get; }

        public SessionOverride(int? tenantId, long? userId)
        {
            TenantId = tenantId;
            UserId = userId;
        }
    }
}