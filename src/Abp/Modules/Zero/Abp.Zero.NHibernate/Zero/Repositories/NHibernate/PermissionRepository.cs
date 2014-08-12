using Abp.Application.Authorization.Permissions;
using Abp.Domain.Repositories.NHibernate;

namespace Abp.Zero.Repositories.NHibernate
{
    public class PermissionRepository : NhRepositoryBase<Permission, long>, IPermissionRepository
    {

    }
}