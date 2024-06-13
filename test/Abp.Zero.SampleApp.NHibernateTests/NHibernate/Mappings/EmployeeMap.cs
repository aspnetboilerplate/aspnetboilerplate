using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.EntityHistory;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class EmployeeMap : EntityMap<Employee>
    {
        public EmployeeMap() : base("Employees")
        {
            Map(f => f.FullName);
            Map(f => f.Department);
            
            this.MapFullAudited();
        }
    }
}