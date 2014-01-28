using Abp.Domain.Repositories;

namespace Abp.Security.Users
{
    public interface IUserLoginRepository<TUser> : IRepository<UserLogin<TUser>, long> where TUser : User
    {

    }
}