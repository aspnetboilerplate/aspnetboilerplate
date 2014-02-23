using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class AbpUserLoginRepository : UserLoginRepository<AbpUser>, IUserLoginRepository
    {

    }
}