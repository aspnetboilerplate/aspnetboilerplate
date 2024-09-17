using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory.Nhibernate;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class FooMap : EntityMap<NhFoo>
    {
        public FooMap() : base("Foos")
        {
            Map(f => f.Audited);
            Map(f => f.NonAudited);
        }
    }
}