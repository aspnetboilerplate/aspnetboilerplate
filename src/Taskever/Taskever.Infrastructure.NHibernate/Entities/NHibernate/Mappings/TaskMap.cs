using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;

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
        }
    }
}
