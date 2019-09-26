using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Advertisement : FullAuditedAggregateRoot
    {
        public string Title { get; set; }

        public Advertisement()
        {
        }
    }
}