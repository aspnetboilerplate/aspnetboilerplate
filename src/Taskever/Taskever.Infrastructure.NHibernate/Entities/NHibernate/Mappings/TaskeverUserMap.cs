using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Security.Users;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class TaskeverUserMap : UserMapBase<TaskeverUser>
    {
        public TaskeverUserMap()
        {
            Map(x => x.ProfileImage);
        }
    }
}
