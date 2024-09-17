using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory.Nhibernate
{
    [Audited]
    public class NhComment : Entity
    {
        public virtual NhPost Post { get; set; }

        public virtual string Content { get; set; }
    }
}
