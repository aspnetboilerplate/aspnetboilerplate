using Abp.Domain.Repositories.NHibernate;
using Abp.Users;

namespace Abp.Zero.Repositories.NHibernate
{
    public class UserLoginRepository : NhRepositoryBase<UserLogin, long>, IUserLoginRepository
    {

    }
}