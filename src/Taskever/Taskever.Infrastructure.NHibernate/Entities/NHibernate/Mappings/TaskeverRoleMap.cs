using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Taskever.Security.Roles;

namespace Taskever.Entities.NHibernate.Mappings
{
    public class TaskeverRoleMap : RoleMapBase<TaskeverRole>
    {
        public TaskeverRoleMap()
        {
            //Add your additional field mappings here
        }
    }
}