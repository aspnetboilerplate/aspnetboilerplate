using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Permissions;

namespace Abp.Zero.Repositories.NHibernate
{
    public class PermissionRepository : NhRepositoryBase<Permission, long>, IPermissionRepository
    {

    }
}