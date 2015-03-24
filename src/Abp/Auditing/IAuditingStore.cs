namespace Abp.Auditing
{
    interface IAuditingStore
    {
        void Save(AuditInfo auditInfo);
    }
}