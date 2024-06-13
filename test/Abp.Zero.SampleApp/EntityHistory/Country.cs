using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Country : FullAuditedEntity
    {
        public virtual string CountryCode { get; set; }
    }
}
