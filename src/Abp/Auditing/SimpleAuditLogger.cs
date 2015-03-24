using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.Auditing
{
    internal class SimpleAuditLogger : IAuditingStore, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public SimpleAuditLogger()
        {
            Logger = NullLogger.Instance;
        }

        public void Save(AuditInfo auditInfo)
        {
            Logger.Info(auditInfo.ToString());
        }
    }
}