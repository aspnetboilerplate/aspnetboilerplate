using Abp.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Data.Repositories
{
    /// <summary>
    /// Used to perform <see cref="User"/> related database operations.
    /// </summary>
    public interface IUserRepository : IRepository<User, int>
    {

    }
}
