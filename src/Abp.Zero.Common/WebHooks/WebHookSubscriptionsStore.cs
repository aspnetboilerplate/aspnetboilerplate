using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;

namespace Abp.WebHooks
{
    /// <summary>
    /// Implements <see cref="IWebHookSubscriptionsStore"/> using repositories.
    /// </summary>
    public class WebHookSubscriptionsStore : IWebHookSubscriptionsStore, ITransientDependency
    {
        private readonly IRepository<WebHookSubscriptionInfo, Guid> _webhookSubscriptionRepository;
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public WebHookSubscriptionsStore(IRepository<WebHookSubscriptionInfo, Guid> webhookSubscriptionRepository)
        {
            _webhookSubscriptionRepository = webhookSubscriptionRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        [UnitOfWork]
        public virtual Task<WebHookSubscriptionInfo> GetAsync(Guid id)
        {
            return _webhookSubscriptionRepository.GetAsync(id);
        }

        [UnitOfWork]
        public virtual WebHookSubscriptionInfo Get(Guid id)
        {
            return _webhookSubscriptionRepository.Get(id);
        }

        [UnitOfWork]
        public virtual async Task InsertAsync(WebHookSubscriptionInfo webHookInfo)
        {
            await _webhookSubscriptionRepository.InsertAsync(webHookInfo);
        }

        [UnitOfWork]
        public virtual void Insert(WebHookSubscriptionInfo webHookInfo)
        {
            _webhookSubscriptionRepository.Insert(webHookInfo);
        }

        [UnitOfWork]
        public virtual async Task UpdateAsync(WebHookSubscriptionInfo webHookSubscription)
        {
            await _webhookSubscriptionRepository.UpdateAsync(webHookSubscription);
        }

        [UnitOfWork]
        public virtual void Update(WebHookSubscriptionInfo webHookSubscription)
        {
            _webhookSubscriptionRepository.Update(webHookSubscription);
        }

        [UnitOfWork]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _webhookSubscriptionRepository.DeleteAsync(id);
        }

        [UnitOfWork]
        public virtual void Delete(Guid id)
        {
            _webhookSubscriptionRepository.Delete(id);
        }

        [UnitOfWork]
        public virtual async Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId, string webHookDefinitionName)
        {
            return await _webhookSubscriptionRepository.GetAllListAsync(s =>
                s.TenantId == tenantId &&
                s.IsActive &&
                s.WebHookDefinitions.Contains("\"" + webHookDefinitionName + "\""));
        }

        [UnitOfWork]
        public virtual List<WebHookSubscriptionInfo> GetAllSubscriptions(int? tenantId, string webHookDefinitionName)
        {
            return _webhookSubscriptionRepository.GetAllList(s =>
               s.TenantId == tenantId &&
               s.IsActive &&
               s.WebHookDefinitions.Contains("\"" + webHookDefinitionName + "\""));
        }

        [UnitOfWork]
        public virtual Task<bool> IsSubscribedAsync(int? tenantId, string webHookName)
        {
            return AsyncQueryableExecuter.AnyAsync(_webhookSubscriptionRepository.GetAll()
                .Where(s =>
                    s.IsActive &&
                    s.TenantId == tenantId &&
                    s.WebHookDefinitions.Contains("\"" + webHookName + "\"")
                ));
        }

        [UnitOfWork]
        public virtual bool IsSubscribed(int? tenantId, string webHookName)
        {
            return _webhookSubscriptionRepository.GetAll()
                .Any(s =>
                    s.IsActive &&
                    s.TenantId == tenantId &&
                    s.WebHookDefinitions.Contains("\"" + webHookName + "\"")
                );
        }

        [UnitOfWork]
        public virtual Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId)
        {
            return _webhookSubscriptionRepository.GetAllListAsync(s => s.TenantId == tenantId);
        }

        [UnitOfWork]
        public virtual List<WebHookSubscriptionInfo> GetAllSubscriptions(int? tenantId)
        {
            return _webhookSubscriptionRepository.GetAllList(s => s.TenantId == tenantId);
        }
    }
}
