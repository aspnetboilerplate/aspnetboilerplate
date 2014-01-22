using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Application.Authorization.Permissions
{
    public interface IPermissionManager : ISingletonDependency, IMustInitialize
    {
        Permission GetPermissionOrNull(string permissionName);

        //bool IsPermissionDefined(string permissionName);
    }
}
