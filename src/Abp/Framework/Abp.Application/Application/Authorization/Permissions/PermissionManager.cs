using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Abp.Modules;
using Abp.Utils.Extensions.Collections;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionManager : IPermissionManager
    {
        private readonly IDictionary<string, Permission> _permissions;

        private bool _isInitialized;

        public PermissionManager()
        {
            _permissions = new Dictionary<string, Permission>();
        }

        public Permission GetPermissionOrNull(string permissionName)
        {
            return _permissions.GetOrDefault(permissionName);
        }

        public void Initialize() //TODO: Call initialize from constructor?
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPermissionProvider).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        var provider = (IPermissionProvider)Activator.CreateInstance(type);
                        var permissions = provider.GetPermissions();
                        foreach (var permission in permissions)
                        {
                            _permissions[permission.Name] = permission;
                        }
                    }
                }
            }
        }
    }
}