using Abp.Modules.Core.Entities.NHibernate.Mappings;
using FluentNHibernate.Mapping;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Enums;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class ActivityMap : EntityMap<Activity, long>
    {
        public ActivityMap()
            : base("TeActivities")
        {
            DiscriminateSubClassesOnColumn("ActivityType");
            //Map(x => x.ActivityType).CustomType<ActivityType>().Not.Nullable();
            Map(x => x.CreationTime);
        }
    }

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