using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Application.Authorization.Permissions
{
    public interface IPermissionManager
    {
        Permission GetPermissionOrNull(string permissionName);

        //bool IsPermissionDefined(string permissionName);
    }
}
