using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Foo : Entity
    {
        [Audited]
        public virtual string Audited { get; set; }

        public virtual string NonAudited { get; set; }
    }
}