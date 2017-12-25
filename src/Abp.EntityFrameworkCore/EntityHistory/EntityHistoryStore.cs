using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace Abp.EntityHistory
{
    /// <summary>
    /// Implements <see cref="IEntityHistoryStore"/> to save entity change informations to database.
    /// </summary>
    public class EntityHistoryStore : IEntityHistoryStore, ITransientDependency
    {
        private readonly IRepository<EntityChangeInfo, long> _entityChangeRepository;

        /// <summary>
        /// Creates a new <see cref="EntityHistoryStore"/>.
        /// </summary>
        public EntityHistoryStore(IRepository<EntityChangeInfo, long> entityChangeRepository)
        {
            _entityChangeRepository = entityChangeRepository;
        }

        public virtual Task SaveAsync(EntityChangeInfo entityChangeInfo)
        {
            return _entityChangeRepository.InsertAsync(entityChangeInfo);
        }
    }
}
