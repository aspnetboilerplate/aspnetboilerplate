using System.Collections.Generic;
using Abp.Startup;

namespace Abp.Authorization.Permissions
{
    /// <summary>
    /// Used to store and manipulate dictionary of permissions.
    /// </summary>
    internal class PermissionDictionary : Dictionary<string, Permission>
    {
        /// <summary>
        /// Adds permissions to dictionary in given group and it's all child groups recursively.
        /// </summary>
        public void AddGroupPermissionsRecursively(PermissionGroup permissionGroup)
        {
            //Add permissions in given group
            foreach (var permission in permissionGroup.Permissions)
            {
                AddPermissionRecursively(permission);
            }

            //Add permissions in all child groups recursively
            foreach (var childGroup in permissionGroup.Children)
            {
                AddGroupPermissionsRecursively(childGroup);
            }
        }

        /// <summary>
        /// Adds a permission and it's all child permissions to dictionary.
        /// </summary>
        /// <param name="permission"></param>
        private void AddPermissionRecursively(Permission permission)
        {
            //Prevent multiple adding of same named permission.
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