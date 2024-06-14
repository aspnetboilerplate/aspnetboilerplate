using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory.Nhibernate;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class EmployeeMap : EntityMap<NhEmployee>
    {
        public EmployeeMap() : base("Employees")
        {
            Map(f => f.FullName);
            Map(f => f.Department);
            
            this.MapFullAudited();
        }
    }
}