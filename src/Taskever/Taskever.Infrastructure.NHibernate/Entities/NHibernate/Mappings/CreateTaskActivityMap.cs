using FluentNHibernate.Mapping;
using Taskever.Activities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class CreateTaskActivityMap : SubclassMap<CreateTaskActivity>
    {
        public CreateTaskActivityMap()
        {
            DiscriminatorValue((int)ActivityType.CreateTask);
            References(x => x.CreatorUser).Column("CreatorUserId").Not.Nullable().Not.LazyLoad();
            References(x => x.AssignedUser).Column("AssignedUserId").Not.Nullable().Not.LazyLoad();
            References(x => x.Task).Column("TaskId").Not.Nullable().Not.LazyLoad();
        }
    }
}