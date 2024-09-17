using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory.Nhibernate;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class CountryMap : EntityMap<NhCountry>
    {
        public CountryMap() : base("Countries")
        {
            Map(f => f.CountryCode);
            this.MapFullAudited();
        }
    }
}