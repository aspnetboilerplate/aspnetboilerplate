using Abp.Domain.Repositories;

namespace Abp.Security.Users
{
    public interface IUserLoginRepository<TUser> : IRepository<UserLogin, long> where TUser : AbpUser
    {

    }
}