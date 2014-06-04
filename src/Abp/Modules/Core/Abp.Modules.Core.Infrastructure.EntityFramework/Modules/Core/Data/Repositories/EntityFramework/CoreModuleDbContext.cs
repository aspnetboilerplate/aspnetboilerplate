using System.Data.Entity;
using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Permissions;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class CoreModuleDbContext : AbpDbContext
    {
        public virtual IDbSet<Permission> Permissions { get; set; }
    }
}