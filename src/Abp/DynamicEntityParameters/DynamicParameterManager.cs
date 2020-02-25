using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;

namespace Abp.DynamicEntityParameters
{
    public class DynamicParameterManager : IDynamicParameterManager, ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDynamicParameterStore _dynamicParameterStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public const string CacheName = "AbpZeroDynamicParameterCache";
        private ITypedCache<int, DynamicParameter> DynamicParameterCache => _cacheManager.GetCache<int, DynamicParameter>(CacheName);

        public DynamicParameterManager(
            ICacheManager cacheManager,
            IDynamicParameterStore dynamicParameterStore,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _cacheManager = cacheManager;
            _dynamicParameterStore = dynamicParameterStore;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public DynamicParameter Get(int id)
        {
            return DynamicParameterCache.Get(id, () => _dynamicParameterStore.Get(id));
        }

        public Task<DynamicParameter> GetAsync(int id)
        {
            return DynamicParameterCache.GetAsync(id, (i) => _dynamicParameterStore.GetAsync(id));
        }

        public void Add(DynamicParameter dynamicParameter)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicParameterStore.Add(dynamicParameter);
                uow.Complete();
            }

            DynamicParameterCache.Set(dynamicParameter.Id, dynamicParameter);
        }

        public async Task AddAsync(DynamicParameter dynamicParameter)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicParameterStore.AddAsync(dynamicParameter);
                await uow.CompleteAsync();
            }

            await DynamicParameterCache.SetAsync(dynamicParameter.Id, dynamicParameter);
        }

        public void Update(DynamicParameter dynamicParameter)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicParameterStore.Update(dynamicParameter);
                uow.Complete();
            }

            DynamicParameterCache.Set(dynamicParameter.Id, dynamicParameter);
        }

        public async Task UpdateAsync(DynamicParameter dynamicParameter)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicParameterStore.UpdateAsync(dynamicParameter);
                await uow.CompleteAsync();
            }

            await DynamicParameterCache.SetAsync(dynamicParameter.Id, dynamicParameter);
        }

        public void Delete(int id)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicParameterStore.Delete(id);
                uow.Complete();
            }

            DynamicParameterCache.Remove(id);
        }

        public async Task DeleteAsync(int id)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicParameterStore.DeleteAsync(id);
                await uow.CompleteAsync();
            }

            await DynamicParameterCache.RemoveAsync(id);
        }
    }
}
