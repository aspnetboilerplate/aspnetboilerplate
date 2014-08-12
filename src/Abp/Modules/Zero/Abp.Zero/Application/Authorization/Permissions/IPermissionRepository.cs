using Abp.Domain.Repositories;

namespace Abp.Application.Authorization.Permissions
{
    public interface IPermissionRepository : IRepository<Permission, long>
    {

    }
}
