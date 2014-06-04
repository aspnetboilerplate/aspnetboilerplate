using Abp.Domain.Uow;
using Abp.Security.Permissions;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class PermissionRepository : CoreModuleEfRepositoryBase<Permission, long>, IPermissionRepository
    {

    }
}