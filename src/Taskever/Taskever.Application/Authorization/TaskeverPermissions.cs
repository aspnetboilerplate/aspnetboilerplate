using System.Collections.Generic;
using Abp.Application.Authorization.Permissions;

namespace Taskever.Authorization
{
    public class TaskeverPermissions : IPermissionProvider
    {
        public const string CreateTask = "Taskever.Tasks.Create";

        private static readonly Permission[] AllPermissions;

        static TaskeverPermissions()
        {
            AllPermissions = new[]
                             {
                                 new Permission(CreateTask, "CreateTaskPermissionDisplayName")
                             }; 
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return AllPermissions;
        }
    }
}
