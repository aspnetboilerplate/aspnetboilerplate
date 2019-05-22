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
        private readonly IRepository<EntityChangeSet, long> _changeSetRepository;

        /// <summary>
        /// Creates a new <see cref="EntityHistoryStore"/>.
        /// </summary>
        public EntityHistoryStore(IRepository<EntityChangeSet, long> changeSetRepository)
        {
            _changeSetRepository = changeSetRepository;
        }

        public virtual Task SaveAsync(EntityChangeSet changeSet)
        {
            return _changeSetRepository.InsertAsync(changeSet);
        }

        public virtual void Save(EntityChangeSet changeSet)
        {
            _changeSetRepository.Insert(changeSet);
        }
    }
}
