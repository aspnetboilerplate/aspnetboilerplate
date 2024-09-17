using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory.Nhibernate
{
    public class NhCountry : FullAuditedEntity
    {
        public virtual string CountryCode { get; set; }
    }
}
