using System.Collections.Generic;
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
        private readonly IRepository<EntityChangeInfo, long> _entityChangeRepository;
        private readonly IRepository<EntityPropertyChangeInfo, long> _propertyChangeRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Creates a new <see cref="EntityHistoryStore"/>.
        /// </summary>
        public EntityHistoryStore(
            IRepository<EntityChangeSet, long> changeSetRepository, 
            IRepository<EntityChangeInfo, long> entityChangeRepository,
            IRepository<EntityPropertyChangeInfo, long> propertyChangeRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _changeSetRepository = changeSetRepository;
            _entityChangeRepository = entityChangeRepository;
            _propertyChangeRepository = propertyChangeRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected async Task SaveAsync(EntityChangeInfo entityChangeInfo)
        {
            var entityChangeId = await _entityChangeRepository.InsertAndGetIdAsync(entityChangeInfo);
            await SaveAsync(entityChangeInfo.PropertyChanges, entityChangeId);
        }

        public virtual async Task SaveAsync(EntityChangeSet changeSet)
        {
            var changeSetId = await _changeSetRepository.InsertAndGetIdAsync(changeSet);
            await SaveAsync(changeSet.EntityChanges, changeSetId);
        }

        protected async Task SaveAsync(EntityPropertyChangeInfo propertyChangeInfo)
        {
            await _propertyChangeRepository.InsertAsync(propertyChangeInfo);
        }

        private async Task SaveAsync(IList<EntityChangeInfo> entityChangeInfos, long changeSetId)
        {
            foreach (var entityChangeInfo in entityChangeInfos)
            {
                entityChangeInfo.EntityChangeSetId = changeSetId;
                await SaveAsync(entityChangeInfo);
            }
        }

        private async Task SaveAsync(ICollection<EntityPropertyChangeInfo> propertyChangeInfos, long entityChangeId)
        {
            foreach (var propertyChange in propertyChangeInfos)
            {
                propertyChange.EntityChangeId = entityChangeId;
                await SaveAsync(propertyChange);
            }

            // Save once for all properties
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }
    }
}
