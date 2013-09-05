using Abp.Modules.Core.Entities.NHibernate.Mappings;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class TaskMap : EntityMap<Task>
    {
        public TaskMap()
            : base("TeTasks")
        {
            References(x => x.Tenant).Column("TenantId");
            Map(x => x.Title).Not.Nullable();
            Map(x => x.Description).Nullable();
        }
    }
}
