using System.Collections.Generic;
using Abp.Application.Authorization.Permissions;
using Abp.Localization;
using Taskever.Localization.Resources;

namespace Taskever.Authorization
{
    public class TaskeverPermissions : IPermissionDefinitionProvider
    {
        public const string CreateTask = "Taskever.Tasks.Create";

        private static readonly PermissionDefinition[] AllPermissionsDefinition;

        static TaskeverPermissions()
        {
            AllPermissionsDefinition =
                new[]
                {
                    new PermissionDefinition(
                        CreateTask,
                        new LocalizableString("CreateTaskPermissionDisplayName", TaskeverLocalizationSource.SourceName)
                        )
                };
        }

        public IEnumerable<PermissionDefinition> GetPermissions(PermissionDefinitionProviderContext context)
        {
            return AllPermissionsDefinition;
        }
    }
}
