using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Reaction : FullAuditedAggregateRoot
    {
        public string Name { get; set; }
    }
}
