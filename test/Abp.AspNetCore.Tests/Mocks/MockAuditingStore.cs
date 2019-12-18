using System.Collections.Concurrent;
using System.Threading.Tasks;
using Abp.Auditing;

namespace Abp.AspNetCore.Mocks
{
    public class MockAuditingStore : IAuditingStore
    {
        public ConcurrentBag<AuditInfo> Logs { get; set; }

        public MockAuditingStore()
        {
            Logs = new ConcurrentBag<AuditInfo>();
        }

        public Task SaveAsync(AuditInfo auditInfo)
        {
            Logs.Add(auditInfo);
            return Task.FromResult(0);
        }

        public void Save(AuditInfo auditInfo)
        {
            Logs.Add(auditInfo);
        }
    }
}
