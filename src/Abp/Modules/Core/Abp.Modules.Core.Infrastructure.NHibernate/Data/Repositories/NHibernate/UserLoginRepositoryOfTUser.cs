using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class UserLoginRepository<TUser> : NhRepositoryBase<UserLogin, long>, IUserLoginRepository<TUser> where TUser : AbpUser
    {

    }
}