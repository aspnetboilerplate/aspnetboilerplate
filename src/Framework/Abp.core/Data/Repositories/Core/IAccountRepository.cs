using Abp.Entities;
using Abp.Entities.Core;

namespace Abp.Data.Repositories.Core
{
    /// <summary>
    /// Used to perform <see cref="Account"/> related database operations.
    /// </summary>
    public interface IAccountRepository : IRepository<Account, int>
    {

    }
}