using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    public class Foo : Entity
    {
        [Audited]
        public string Audited { get; set; }

        public string NonAudited { get; set; }
    }
}
