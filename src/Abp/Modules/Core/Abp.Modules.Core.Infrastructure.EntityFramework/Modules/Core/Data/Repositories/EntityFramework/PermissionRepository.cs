using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Permissions;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class PermissionRepository : EfRepositoryBase<Permission, long>, IPermissionRepository
    {

    }
}