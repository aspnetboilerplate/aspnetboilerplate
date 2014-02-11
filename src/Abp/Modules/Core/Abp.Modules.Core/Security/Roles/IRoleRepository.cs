using System.Collections.Generic;
using Abp.Domain.Repositories;

namespace Abp.Security.Roles
{
    public interface IRoleRepository<TRole> : IRepository<TRole> where TRole : AbpRole
    {
        List<TRole> GetAllListWithPermissions();
    }
}