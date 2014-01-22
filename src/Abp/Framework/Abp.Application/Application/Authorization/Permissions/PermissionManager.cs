using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Abp.Utils.Extensions.Collections;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionManager : IPermissionManager, IMustInitialize
    {
        private readonly IDictionary<string, Permission> _permissions;

        public PermissionManager()
        {
            _permissions = new Dictionary<string, Permission>();
        }

        public Permission GetPermissionOrNull(string permissionName)
        {
            return _permissions.GetOrDefault(permissionName);
        }

        //public bool IsPermissionDefined(string permissionName)
        //{
        //    return _permissions.ContainsKey(permissionName);
        //}

        public void Initialize()
        {
            //TODO: Scan all assemblies and add permissions to _permissions dictionary!
        }
    }
}