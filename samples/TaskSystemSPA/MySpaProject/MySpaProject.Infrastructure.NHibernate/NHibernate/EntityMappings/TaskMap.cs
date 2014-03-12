using Abp.Domain.Entities.Mapping;
using MySpaProject.Tasks;

namespace MySpaProject.NHibernate.EntityMappings
{
    public class TaskMap : EntityMap<Task, long>
    {
        public TaskMap()
            : base("TsTasks")
        {
            Map(x => x.Description);
            Map(x => x.CreationTime);
            Map(x => x.State).CustomType<TaskState>();
            References(x => x.AssignedPerson).Column("AssignedPersonId").LazyLoad();
        }
    }
}
