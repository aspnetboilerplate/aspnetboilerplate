using Abp.Domain.Repositories;

namespace Abp.Users
{
    public interface IUserLoginRepository : IRepository<UserLogin, long>
    {

    }
}