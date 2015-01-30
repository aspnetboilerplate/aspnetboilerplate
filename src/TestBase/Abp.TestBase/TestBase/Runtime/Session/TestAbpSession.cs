using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.TestBase.Runtime.Session
{
    public class TestAbpSession : IAbpSession, ISingletonDependency
    {
        public long? UserId { get; set; }

        public int? TenantId { get; set; }
    }
}