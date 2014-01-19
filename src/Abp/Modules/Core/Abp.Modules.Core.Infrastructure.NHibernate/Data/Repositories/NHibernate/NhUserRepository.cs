using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using Abp.Users;
using NHibernate.Linq;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    /// <summary>
    /// Implements <see cref="IUserRepository"/> for NHibernate.
    /// </summary>
    public class NhUserRepository : NhRepositoryBase<User>, IUserRepository
    {

    }
}
