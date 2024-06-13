using Abp.Zero.SampleApp.EntityHistory.Nhibernate;
using FluentNHibernate.Mapping;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class CategoryMap : ClassMap<NhCategory>
    {
        public CategoryMap()
        {
            Table("Categories");
            Id(c => c.Id);
            Map(c => c.DisplayName);
        }
    }
}