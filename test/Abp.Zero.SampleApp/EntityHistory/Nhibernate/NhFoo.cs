using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory.Nhibernate
{
    public class NhFoo : Entity
    {
        [Audited]
        public virtual string Audited { get; set; }

        public virtual string NonAudited { get; set; }
    }
}