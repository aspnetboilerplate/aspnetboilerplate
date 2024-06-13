using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class FooMap : EntityMap<Foo>
    {
        public FooMap() : base("Foos")
        {
            Map(f => f.Audited);
            Map(f => f.NonAudited);
        }
    }
}