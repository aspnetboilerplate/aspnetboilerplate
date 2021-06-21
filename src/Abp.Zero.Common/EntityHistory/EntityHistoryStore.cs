using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.EntityHistory
{
    /// <summary>
    /// Implements <see cref="IEntityHistoryStore"/> to save entity change informations to database.
    /// </summary>
    public class EntityHistoryStore : IEntityHistoryStore, ITransientDependency
    {
        private readonly IRepository<EntityChangeSet, long> _changeSetRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Creates a new <see cref="EntityHistoryStore"/>.
        /// </summary>
        public EntityHistoryStore(
            IRepository<EntityChangeSet, long> changeSetRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _changeSetRepository = changeSetRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task SaveAsync(EntityChangeSet changeSet)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _changeSetRepository.InsertAsync(changeSet);
            });
        }

        public virtual void Save(EntityChangeSet changeSet)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _changeSetRepository.Insert(changeSet);
            });
        }
    }
}
