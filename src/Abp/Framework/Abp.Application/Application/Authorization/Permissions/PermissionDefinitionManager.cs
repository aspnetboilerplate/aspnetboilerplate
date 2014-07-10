using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Utils.Extensions.Collections;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionDefinitionManager : IPermissionDefinitionManager
    {
        private readonly IDictionary<string, PermissionDefinition> _permissions;

        public PermissionDefinitionManager()
        {
            _permissions = new Dictionary<string, PermissionDefinition>();
            Initialize();
        }

        public PermissionDefinition GetPermissionOrNull(string permissionName)
        {
            return _permissions.GetOrDefault(permissionName);
        }

        public IReadOnlyList<PermissionDefinition> GetAllPermissions()
        {
            return _permissions.Values.ToImmutableList();
        }

        private void Initialize()
        {
            var context = new PermissionDefinitionProviderContext();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPermissionDefinitionProvider).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        var provider = (IPermissionDefinitionProvider)Activator.CreateInstance(type);
                        foreach (var permission in provider.GetPermissions(context))
                        {
                            _permissions[permission.Name] = permission;
                        }
                    }
                }
            }
        }
    }
}