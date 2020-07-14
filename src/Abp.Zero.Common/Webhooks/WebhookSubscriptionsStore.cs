using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq;

namespace Abp.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookSubscriptionsStore"/> using repositories.
    /// </summary>
    public class WebhookSubscriptionsStore : IWebhookSubscriptionsStore, ITransientDependency
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<WebhookSubscriptionInfo, Guid> _webhookSubscriptionRepository;

        public WebhookSubscriptionsStore(
            IRepository<WebhookSubscriptionInfo, Guid> webhookSubscriptionRepository
            )
        {
            _webhookSubscriptionRepository = webhookSubscriptionRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }
        
        public virtual Task<WebhookSubscriptionInfo> GetAsync(Guid id)
        {
            return _webhookSubscriptionRepository.GetAsync(id);
        }
        
        public virtual WebhookSubscriptionInfo Get(Guid id)
        {
            return _webhookSubscriptionRepository.Get(id);
        }
        
        public virtual async Task InsertAsync(WebhookSubscriptionInfo webhookInfo)
        {
            await _webhookSubscriptionRepository.InsertAsync(webhookInfo);
        }
        
        public virtual void Insert(WebhookSubscriptionInfo webhookInfo)
        {
            _webhookSubscriptionRepository.Insert(webhookInfo);
        }
        
        public virtual async Task UpdateAsync(WebhookSubscriptionInfo webhookSubscription)
        {
            await _webhookSubscriptionRepository.UpdateAsync(webhookSubscription);
        }
        
        public virtual void Update(WebhookSubscriptionInfo webhookSubscription)
        {
            _webhookSubscriptionRepository.Update(webhookSubscription);
        }
        
        public virtual async Task DeleteAsync(Guid id)
        {
            await _webhookSubscriptionRepository.DeleteAsync(id);
        }
        
        public virtual void Delete(Guid id)
        {
            _webhookSubscriptionRepository.Delete(id);
        }
        
        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId, string webhookDefinitionName)
        {
            return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                subscriptionInfo.TenantId == tenantId &&
                subscriptionInfo.IsActive &&
                subscriptionInfo.Webhooks.Contains("\"" + webhookDefinitionName + "\""));
        }
        
        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptions(int? tenantId, string webhookDefinitionName)
        {
            return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
               subscriptionInfo.TenantId == tenantId &&
               subscriptionInfo.IsActive &&
               subscriptionInfo.Webhooks.Contains("\"" + webhookDefinitionName + "\""));
        }
        
        public virtual Task<bool> IsSubscribedAsync(int? tenantId, string webhookName)
        {
            return AsyncQueryableExecuter.AnyAsync(_webhookSubscriptionRepository.GetAll()
                .Where(subscriptionInfo =>
                    subscriptionInfo.TenantId == tenantId &&
                    subscriptionInfo.IsActive &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                ));
        }
        
        public virtual bool IsSubscribed(int? tenantId, string webhookName)
        {
            return _webhookSubscriptionRepository.GetAll()
                .Any(subscriptionInfo =>
                    subscriptionInfo.TenantId == tenantId &&
                    subscriptionInfo.IsActive &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
        }
        
        public virtual Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId)
        {
            return _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo => subscriptionInfo.TenantId == tenantId);
        }
        
        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptions(int? tenantId)
        {
            return _webhookSubscriptionRepository.GetAllList(subscriptionInfo => subscriptionInfo.TenantId == tenantId);
        }
    }
}
