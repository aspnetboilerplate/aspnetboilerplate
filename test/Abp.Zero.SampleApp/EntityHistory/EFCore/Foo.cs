using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory.EFCore
{
    public class Foo : Entity
    {
        [Audited]
        public string Audited { get; set; }

        public string NonAudited { get; set; }
    }
}