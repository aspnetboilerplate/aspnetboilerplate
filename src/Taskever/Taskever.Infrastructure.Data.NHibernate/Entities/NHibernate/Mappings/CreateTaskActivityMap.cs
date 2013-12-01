using FluentNHibernate.Mapping;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class CreateTaskActivityMap : SubclassMap<CreateTaskActivity>
    {
        public CreateTaskActivityMap()
        {
            DiscriminatorValue((int)ActivityType.CreateTask);
            References(x => x.CreatorUser).Column("CreatorUserId").Not.Nullable();//.LazyLoad(Laziness.False);
            References(x => x.AssignedUser).Column("AssignedUserId").Not.Nullable();//.LazyLoad(Laziness.False);
            References(x => x.Task).Column("TaskId").Not.Nullable();//.LazyLoad(Laziness.False);
        }
    }
}