using System;
using System.Collections.Generic;
using Abp.Utils.Extensions.Collections;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionManager : IPermissionManager
    {
        private readonly IDictionary<string, Permission> _permissions;

        public PermissionManager()
        {
            _permissions = new Dictionary<string, Permission>();
            Initialize();
        }

        public Permission GetPermissionOrNull(string permissionName)
        {
            return _permissions.GetOrDefault(permissionName);
        }

        private void Initialize()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPermissionProvider).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        var provider = (IPermissionProvider)Activator.CreateInstance(type);
                        foreach (var permission in provider.GetPermissions())
                        {
                            _permissions[permission.Name] = permission;
                        }
                    }
                }
            }
        }
    }
}