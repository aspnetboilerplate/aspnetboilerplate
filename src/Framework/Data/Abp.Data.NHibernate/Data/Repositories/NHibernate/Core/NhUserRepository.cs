using Abp.Data.Repositories.Core;
using Abp.Entities.Core;

namespace Abp.Data.Repositories.NHibernate.Core
{
    /// <summary>
    /// Implements <see cref="IUserRepository"/> for NHibernate.
    /// </summary>
    public class NhUserRepository : NhRepositoryBase<User, int>, IUserRepository
    {

    }
}
