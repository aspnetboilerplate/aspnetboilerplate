using Castle.Core.Logging;

namespace Abp.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditingStore"/> to simply write audits to logs.
    /// </summary>
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