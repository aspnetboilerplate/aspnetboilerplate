using Abp.Auditing;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class AuditLogMap : EntityMap<AuditLog, long>
{
    public AuditLogMap()
        : base("AbpAuditLogs")
    {
        Map(x => x.BrowserInfo)
            .Length(AuditLog.MaxBrowserInfoLength);
        Map(x => x.ClientIpAddress)
            .Length(AuditLog.MaxClientIpAddressLength);
        Map(x => x.ClientName)
            .Length(AuditLog.MaxClientNameLength);
        Map(x => x.CustomData)
            .Length(AuditLog.MaxCustomDataLength);
        Map(x => x.Exception)
            .Length(AuditLog.MaxExceptionLength);
        Map(x => x.ExecutionDuration)
            .Not.Nullable();
        Map(x => x.ExecutionTime)
            .Not.Nullable();
        Map(x => x.ImpersonatorUserId);
        Map(x => x.ImpersonatorTenantId);
        Map(x => x.MethodName)
            .Length(AuditLog.MaxMethodNameLength);
        Map(x => x.Parameters)
            .Length(AuditLog.MaxParametersLength);
        Map(x => x.ServiceName)
            .Length(AuditLog.MaxServiceNameLength);
        Map(x => x.TenantId);
        Map(x => x.UserId);
        Map(x => x.ReturnValue)
            .Length(Extensions.NvarcharMax);
        Map(x => x.ExceptionMessage)
            .Length(AuditLog.MaxExceptionMessageLength);



    }
}