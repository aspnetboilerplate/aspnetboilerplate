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
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public WebHookWorkItemStore(IRepository<WebHookWorkItem, Guid> webhookWorkItemRepository)
        {
            _webhookWorkItemRepository = webhookWorkItemRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        [UnitOfWork]
        public Task InsertAsync(WebHookWorkItem webHookWorkItem)
        {
            return _webhookWorkItemRepository.InsertAsync(webHookWorkItem);
        }

        [UnitOfWork]
        public void Insert(WebHookWorkItem webHookWorkItem)
        {
            _webhookWorkItemRepository.Insert(webHookWorkItem);
        }

        [UnitOfWork]
        public Task UpdateAsync(WebHookWorkItem webHookWorkItem)
        {
            return _webhookWorkItemRepository.UpdateAsync(webHookWorkItem);
        }

        [UnitOfWork]
        public void Update(WebHookWorkItem webHookWorkItem)
        {
            _webhookWorkItemRepository.Update(webHookWorkItem);
        }

        [UnitOfWork]
        public Task<WebHookWorkItem> GetAsync(Guid id)
        {
            return _webhookWorkItemRepository.GetAsync(id);
        }

        [UnitOfWork]
        public WebHookWorkItem Get(Guid id)
        {
            return _webhookWorkItemRepository.Get(id);
        }

        [UnitOfWork]
        public Task<int> GetRepetitionCountAsync(Guid webHookId, Guid webHookSubscriptionId)
        {
            return AsyncQueryableExecuter.CountAsync(
                _webhookWorkItemRepository.GetAll()
                    .Where(workItem =>
                        workItem.WebHookId == webHookId &&
                        workItem.WebHookSubscriptionId == webHookSubscriptionId
                    )
            );
        }

        [UnitOfWork]
        public int GetRepetitionCount(Guid webHookId, Guid webHookSubscriptionId)
        {
            return _webhookWorkItemRepository.GetAll()
                .Count(workItem =>
                    workItem.WebHookId == webHookId &&
                    workItem.WebHookSubscriptionId == webHookSubscriptionId);
        }
    }
}
