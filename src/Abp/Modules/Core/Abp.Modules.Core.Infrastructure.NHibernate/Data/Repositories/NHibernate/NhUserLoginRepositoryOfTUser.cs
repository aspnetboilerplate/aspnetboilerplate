using Abp.Domain.Repositories.NHibernate;
using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class NhUserLoginRepository<TUser> : NhRepositoryBase<UserLogin<TUser>, long>, IUserLoginRepository<TUser> where TUser : AbpUser
    {

    }
}