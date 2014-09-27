using System.Collections.Generic;
using Abp.Startup;

namespace Abp.Authorization.Permissions
{
    /// <summary>
    /// Used to store and manipulate dictionary of permissions.
    /// </summary>
    internal class PermissionDictionary : Dictionary<string, Permission>
    {
        public void AddGroupPermissionsRecursively(PermissionGroup permissionGroup)
        {
            //Add permissions of the group
            foreach (var permission in permissionGroup.Permissions)
            {
                AddPermissionRecursively(permission);
            }

            //Add child groups
            foreach (var childGroup in permissionGroup.Children)
            {
                AddGroupPermissionsRecursively(childGroup);
            }
        }

        private void AddPermissionRecursively(Permission permission)
        {
            //Did defined before?
            Permission existingPermission;
            if (TryGetValue(permission.Name, out existingPermission))
            {
                throw new AbpInitializationException("Duplicate permission name detected for " + permission.Name);
            }

            //Add permission
            this[permission.Name] = permission;

            //Add child permissions
            foreach (var childPermission in permission.Children)
            {
                AddPermissionRecursively(childPermission);
            }
        }
    }
}