using System.Collections.Generic;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Permission> GetPermissions();
    }
}