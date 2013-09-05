using Abp.Data.Repositories.NHibernate;
using Abp.Modules.Core.Entities.Core;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    /// <summary>
    /// Implements <see cref="IUserRepository"/> for NHibernate.
    /// </summary>
    public class NhUserRepository : NhRepositoryBase<User, int>, IUserRepository
    {

    }
}
