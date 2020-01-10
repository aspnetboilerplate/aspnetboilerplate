using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;

namespace Abp.WebHooks
{
    /// <summary>
    /// Implements <see cref="IWebHookWorkItemStore"/> using repositories.
    /// </summary>
    public class WebHookWorkItemStore : IWebHookWorkItemStore, ITransientDependency
    {
        private readonly IRepository<WebHookWorkItem, Guid> _webhookWorkItemRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public WebHookWorkItemStore(IRepository<WebHookWorkItem, Guid> webhookWorkItemRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookWorkItemRepository = webhookWorkItemRepository;
            _unitOfWorkManager = unitOfWorkManager;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        [UnitOfWork]
        public virtual async Task InsertAsync(WebHookWorkItem webHookWorkItem)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webHookWorkItem.TenantId))
            {
                await _webhookWorkItemRepository.InsertAsync(webHookWorkItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void Insert(WebHookWorkItem webHookWorkItem)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webHookWorkItem.TenantId))
            {
                _webhookWorkItemRepository.Insert(webHookWorkItem);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateAsync(WebHookWorkItem webHookWorkItem)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webHookWorkItem.TenantId))
            {
                await _webhookWorkItemRepository.UpdateAsync(webHookWorkItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void Update(WebHookWorkItem webHookWorkItem)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webHookWorkItem.TenantId))
            {
                _webhookWorkItemRepository.Update(webHookWorkItem);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task<WebHookWorkItem> GetAsync(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await _webhookWorkItemRepository.GetAsync(id);
            }
        }

        [UnitOfWork]
        public virtual WebHookWorkItem Get(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookWorkItemRepository.Get(id);
            }
        }

        [UnitOfWork]
        public virtual async Task<int> GetRepetitionCountAsync(int? tenantId, Guid webHookId, Guid webHookSubscriptionId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await AsyncQueryableExecuter.CountAsync(
                    _webhookWorkItemRepository.GetAll()
                        .Where(workItem =>
                            workItem.WebHookId == webHookId &&
                            workItem.WebHookSubscriptionId == webHookSubscriptionId
                        )
                );
            }
        }

        [UnitOfWork]
        public virtual int GetRepetitionCount(int? tenantId, Guid webHookId, Guid webHookSubscriptionId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookWorkItemRepository.GetAll()
                    .Count(workItem =>
                        workItem.WebHookId == webHookId &&
                        workItem.WebHookSubscriptionId == webHookSubscriptionId);
            }
        }
    }
}
