using Abp.NHibernate.EntityMappings;

namespace Abp.NHibernate.Tests.Entities
{
    public class HotelMap : EntityMap<Hotel>
    {
        public HotelMap() : base("Hotels")
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.CreationDate);
            Map(x => x.ModificationDate).Nullable();

            Component(h => h.Headquarter,
                l =>
                {
                    l.Map(x => x.Name, "Headquarter_Name");
                    l.Map(x => x.CreationDate, "Headquarter_CreationDate");
                    l.Map(x => x.ModificationDate, "Headquarter_ModificationDate").Nullable();
                });
        }
    }
}