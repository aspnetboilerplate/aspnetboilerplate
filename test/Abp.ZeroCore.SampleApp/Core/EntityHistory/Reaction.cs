using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    public class Reaction : FullAuditedAggregateRoot
    {
        public string Name { get; set; }
    }
}
