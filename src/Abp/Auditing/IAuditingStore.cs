namespace Abp.Auditing
{
    public interface IAuditingStore
    {
        void Save(AuditInfo auditInfo);
    }
}