using Abp.Domain.Repositories;

namespace Abp.Security.Users
{
    public interface IUserLoginRepository : IRepository<UserLogin, long>
    {

    }
}