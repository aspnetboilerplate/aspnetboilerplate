using Abp.Domain.Entities.Mapping;
using Taskever.Tasks;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class TaskMap : EntityMap<Task>
    {
        public TaskMap()
            : base("TeTasks")
        {
            Map(x => x.Title).Not.Nullable();
            Map(x => x.Description).Nullable();
            References(x => x.AssignedUser).Column("AssignedUserId").LazyLoad();
            Map(x => x.Priority).CustomType<TaskPriority>().Not.Nullable();
            Map(x => x.Privacy).CustomType<TaskPrivacy>().Not.Nullable();
            Map(x => x.State).CustomType<TaskState>().Not.Nullable();
            this.MapAudited();
        }
    }
}
