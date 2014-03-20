using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class UserLoginRepository : NhRepositoryBase<UserLogin, long>, IUserLoginRepository
    {

    }
}