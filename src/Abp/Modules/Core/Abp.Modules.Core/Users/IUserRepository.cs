using Abp.Domain.Repositories;

namespace Abp.Users
{
    /// <summary>
    /// Used to perform <see cref="User"/> related database operations.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {

    }
}
