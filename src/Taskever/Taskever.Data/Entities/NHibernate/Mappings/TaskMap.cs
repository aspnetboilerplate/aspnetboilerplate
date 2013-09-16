using Abp.Modules.Core.Entities.NHibernate.Mappings;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class TaskMap : EntityMap<Task>
    {
        public TaskMap()
            : base("TeTasks")
        {
            Map(x => x.Title).Not.Nullable();
            Map(x => x.Description).Nullable();
            Map(x => x.Priority).CustomType<TaskPriority>().Not.Nullable();
        }
    }
}
