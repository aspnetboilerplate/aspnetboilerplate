using Castle.Core.Logging;

namespace Abp.Auditing
{
    internal class SimpleLogAuditingStore : IAuditingStore
    {
        public ILogger Logger { get; set; }

        public SimpleLogAuditingStore()
        {
            Logger = NullLogger.Instance;
        }

        public void Save(AuditInfo auditInfo)
        {
            Logger.Info(auditInfo.ToString());
        }
    }
}