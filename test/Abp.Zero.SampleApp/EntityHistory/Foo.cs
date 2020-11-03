using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Foo : Entity
    {
        [Audited]
        public string Audited { get; set; }

        public string NonAudited { get; set; }
    }
}