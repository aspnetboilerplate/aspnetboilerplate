using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory.Nhibernate;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class AdvertisementMap : EntityMap<NhAdvertisement>
    {
        public AdvertisementMap() : base("Advertisements")
        {
            Map(f => f.Banner);
        }
    }
}