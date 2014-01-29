using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class AbpUserRepository : NhUserRepository<AbpUser>, IAbpUserRepository
    {

    }
}