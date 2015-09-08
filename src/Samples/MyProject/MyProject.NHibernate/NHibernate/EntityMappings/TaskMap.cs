using Abp.NHibernate.EntityMappings;
using MyProject.People;
using MyProject.Tasks;

namespace MyProject.NHibernate.EntityMappings
{
    /// <summary>
    /// Define mapping between <see cref="Task"/> entity and "StsTasks" table using FluentNHibernate.
    /// 
    /// Inherits from <see cref="EntityMap{TEntity,TPrimaryKey}"/>.
    /// EntityMap gets table name in it's constructor and automatically maps Id property. So, we do not map Id again.
    /// </summary>
    public class TaskMap : EntityMap<Task, long>
    {
        public TaskMap()
            : base("StsTasks")
        {
            Map(x => x.Description);
            Map(x => x.CreationTime);
            Map(x => x.State).CustomType<TaskState>();
            References(x => x.AssignedPerson).Column("AssignedPersonId").LazyLoad();
        }
    }
}
