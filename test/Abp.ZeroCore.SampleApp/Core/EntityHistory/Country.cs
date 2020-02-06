using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    public class Country : FullAuditedEntity
    {
        public string CountryCode { get; set; }
    }
}
