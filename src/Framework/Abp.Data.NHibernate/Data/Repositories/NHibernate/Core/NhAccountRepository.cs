using Abp.Data.Repositories.Core;
using Abp.Entities.Core;

namespace Abp.Data.Repositories.NHibernate.Core
{
    /// <summary>
    /// Implements <see cref="IAccountRepository"/> for NHibernate.
    /// </summary>
    public class NhAccountRepository : NhRepositoryBase<Account, int>, IAccountRepository
    {

    }
}