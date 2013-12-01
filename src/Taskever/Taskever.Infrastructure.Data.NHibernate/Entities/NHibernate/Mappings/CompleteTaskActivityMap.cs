using FluentNHibernate.Mapping;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class CompleteTaskActivityMap : SubclassMap<CompleteTaskActivity>
    {
        public CompleteTaskActivityMap()
        {
            DiscriminatorValue((int)ActivityType.CompleteTask);
            References(x => x.AssignedUser).Column("AssignedUserId").Not.Nullable().LazyLoad(Laziness.False);
            References(x => x.Task).Column("TaskId").Not.Nullable().LazyLoad(Laziness.False);
        }
    }
}