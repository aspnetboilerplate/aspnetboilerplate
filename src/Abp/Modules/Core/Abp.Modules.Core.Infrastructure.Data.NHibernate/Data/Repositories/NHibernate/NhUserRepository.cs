using System.Linq;
using Abp.Data.Repositories.NHibernate;
using Abp.Modules.Core.Domain.Entities;
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
