using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Permissions;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class PermissionRepository : NhRepositoryBase<Permission, long>, IPermissionRepository
    {

    }
}