using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Dependency.Conventions;

namespace Abp.Application.Authorization.Permissions
{
    public interface IPermissionManager : ISingletonDependency
    {
        Permission GetPermissionOrNull(string permissionName);

        void Initialize();
    }
}
