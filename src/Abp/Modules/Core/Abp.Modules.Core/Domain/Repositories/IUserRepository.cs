using Abp.Domain.Repositories;
using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Domain.Repositories
{
    /// <summary>
    /// Used to perform <see cref="User"/> related database operations.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {

    }
}
