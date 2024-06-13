using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class AdvertisementMap : EntityMap<Advertisement>
    {
        public AdvertisementMap() : base("Advertisements")
        {
            Map(f => f.Banner);
        }
    }
}