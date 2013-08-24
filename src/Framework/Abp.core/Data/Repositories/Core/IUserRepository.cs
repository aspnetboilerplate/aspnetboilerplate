using Abp.Entities.Core;

namespace Abp.Data.Repositories.Core
{
    /// <summary>
    /// Used to perform <see cref="User"/> related database operations.
    /// </summary>
    public interface IUserRepository : IRepository<User, int>
    {

    }
}
