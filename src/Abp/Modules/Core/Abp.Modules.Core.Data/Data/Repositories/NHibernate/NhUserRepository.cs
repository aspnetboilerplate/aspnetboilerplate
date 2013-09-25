using Abp.Data.Repositories.NHibernate;
using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Entities;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    /// <summary>
    /// Implements <see cref="IUserRepository"/> for NHibernate.
    /// </summary>
    public class NhUserRepository : NhRepositoryBase<User, int>, IUserRepository
    {

    }
}
