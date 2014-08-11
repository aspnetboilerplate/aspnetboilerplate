using Abp.Domain.Repositories;

namespace Abp.Security.Permissions
{
    public interface IPermissionRepository : IRepository<Permission, long>
    {

    }
}
