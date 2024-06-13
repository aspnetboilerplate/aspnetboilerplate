using Abp.Zero.SampleApp.EntityHistory;
using FluentNHibernate.Mapping;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Table("Categories");
            Id(c => c.Id);
            Map(c => c.DisplayName);
        }
    }
}