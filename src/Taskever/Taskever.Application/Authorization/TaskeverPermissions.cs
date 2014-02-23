using System.Collections.Generic;
using Abp.Application.Authorization.Permissions;
using Abp.Localization;
using Taskever.Localization.Resources;

namespace Taskever.Authorization
{
    public class TaskeverPermissions : IPermissionProvider
    {
        public const string CreateTask = "Taskever.Tasks.Create";

        private static readonly Permission[] AllPermissions;

        static TaskeverPermissions()
        {
            AllPermissions =
                new[]
                {
                    new Permission(
                        CreateTask,
                        new LocalizableString("CreateTaskPermissionDisplayName", TaskeverLocalizationSource.SourceName)
                        )
                };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return AllPermissions;
        }
    }
}
