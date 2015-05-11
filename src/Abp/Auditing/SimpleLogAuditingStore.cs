using System.Threading.Tasks;
using Castle.Core.Logging;

namespace Abp.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditingStore"/> to simply write audits to logs.
    /// </summary>
    public class SimpleLogAuditingStore : IAuditingStore
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static SimpleLogAuditingStore Instance { get { return SingletonInstance; } }
        private static readonly SimpleLogAuditingStore SingletonInstance = new SimpleLogAuditingStore();

        public ILogger Logger { get; set; }

        public SimpleLogAuditingStore()
        {
            Logger = NullLogger.Instance;
        }

        public Task SaveAsync(AuditInfo auditInfo)
        {
            Logger.Info(auditInfo.ToString());
            return Task.FromResult(0);
        }
    }
}