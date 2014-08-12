using System.Collections.Generic;
using System.Linq;
using Abp.Application.Authorization.Roles;
using Abp.Domain.Repositories.NHibernate;
using NHibernate.Linq;

namespace Abp.Zero.Repositories.NHibernate
{
    public class AbpRoleRepository : NhRepositoryBase<AbpRole>, IAbpRoleRepository
    {
        public List<AbpRole> GetAllListWithPermissions()
        {
            return GetAll().Fetch(role => role.Permissions).ToList();
        }       
    }
}