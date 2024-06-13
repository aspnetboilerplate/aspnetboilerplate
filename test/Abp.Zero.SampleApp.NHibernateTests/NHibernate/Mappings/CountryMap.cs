using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class CountryMap : EntityMap<Country>
    {
        public CountryMap() : base("Countries")
        {
            Map(f => f.CountryCode);
            this.MapFullAudited();
        }
    }
}