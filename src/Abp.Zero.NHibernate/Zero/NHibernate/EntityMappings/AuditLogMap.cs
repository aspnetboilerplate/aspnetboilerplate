using Abp.Auditing;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class AuditLogMap : EntityMap<AuditLog, long>
    {
        public AuditLogMap()
            : base("AbpAuditLogs")
        {
            Map(x => x.TenantId);
            Map(x => x.UserId);
            Map(x => x.ServiceName);
            Map(x => x.MethodName);
            Map(x => x.Parameters);
            Map(x => x.ExecutionTime);
            Map(x => x.ExecutionDuration);
            Map(x => x.ClientIpAddress);
            Map(x => x.ClientName);
            Map(x => x.BrowserInfo);
            Map(x => x.Exception);
            Map(x => x.ExceptionMessage);
            Map(x => x.ImpersonatorUserId);
            Map(x => x.ImpersonatorTenantId);
            Map(x => x.CustomData);
        }
    }
}