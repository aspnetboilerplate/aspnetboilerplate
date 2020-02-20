using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Country : FullAuditedEntity
    {
        public string CountryCode { get; set; }
    }
}
