using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory.EFCore
{
    public class Country : FullAuditedEntity
    {
        public string CountryCode { get; set; }
    }
}
