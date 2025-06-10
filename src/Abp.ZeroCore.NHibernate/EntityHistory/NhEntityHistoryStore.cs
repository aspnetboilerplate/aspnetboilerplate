using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System.Threading.Tasks;

namespace Abp.EntityHistory;

public class NhEntityHistoryStore : EntityHistoryStore
{
    private readonly IRepository<EntityChangeSet, long> _changeSetRepository;
    private readonly IRepository<EntityChange, long> _entityChangeRepository;
    private readonly IRepository<EntityPropertyChange, long> _entityPropertyChangeRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public NhEntityHistoryStore(IRepository<EntityChangeSet, long> changeSetRepository,
        IRepository<EntityChange, long> entityChangeRepository,
        IRepository<EntityPropertyChange, long> entityPropertyChangeRepository,
        IUnitOfWorkManager unitOfWorkManager) : base(changeSetRepository, unitOfWorkManager)
    {
        _changeSetRepository = changeSetRepository;
        _entityChangeRepository = entityChangeRepository;
        _entityPropertyChangeRepository = entityPropertyChangeRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public override void Save(EntityChangeSet changeSet)
    {
        _unitOfWorkManager.WithUnitOfWork(() =>
        {
            _changeSetRepository.Insert(changeSet);

            foreach (var entityChange in changeSet.EntityChanges)
            {
                entityChange.EntityChangeSetId = changeSet.Id;
                _entityChangeRepository.Insert(entityChange);

                foreach (var propertyChange in entityChange.PropertyChanges)
                {
                    propertyChange.EntityChangeId = entityChange.Id;
                    _entityPropertyChangeRepository.Insert(propertyChange);
                }
            }
        });
    }

    public override async Task SaveAsync(EntityChangeSet changeSet)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            await _changeSetRepository.InsertAsync(changeSet);

            foreach (var entityChange in changeSet.EntityChanges)
            {
                entityChange.EntityChangeSetId = changeSet.Id;
                await _entityChangeRepository.InsertAsync(entityChange);

                foreach (var propertyChange in entityChange.PropertyChanges)
                {
                    propertyChange.EntityChangeId = entityChange.Id;
                    await _entityPropertyChangeRepository.InsertAsync(propertyChange);
                }
            }
        });
    }
}