using System.Collections.Generic;
using Abp.Domain.Repositories;

namespace Abp.Security.Roles
{
    public interface IAbpRoleRepository : IRepository<AbpRole>
    {
        List<AbpRole> GetAllListWithPermissions();
    }
}
