using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Roles;
using NHibernate.Linq;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class AbpRoleRepository : NhRepositoryBase<AbpRole>, IAbpRoleRepository
    {
        public List<AbpRole> GetAllListWithPermissions()
        {
            return GetAll().Fetch(role => role.Permissions).ToList();
        }       
    }
}