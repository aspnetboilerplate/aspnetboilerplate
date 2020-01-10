using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.WebHooks
{
    /// <summary>
    /// Null pattern implementation of <see cref="IWebHookSubscriptionsStore"/>.
    /// It's used if <see cref="IWebHookSubscriptionsStore"/> is not implemented by actual persistent store
    /// </summary>
    public class NullWebHookSubscriptionsStore : IWebHookSubscriptionsStore
    {
        public static NullWebHookSubscriptionsStore Instance { get; } = new NullWebHookSubscriptionsStore();

        public Task<WebHookSubscriptionInfo> GetAsync(Guid id)
        {
            return Task.FromResult<WebHookSubscriptionInfo>(default);
        }

        public WebHookSubscriptionInfo Get(Guid id)
        {
            return default;
        }

        public Task InsertAsync(WebHookSubscriptionInfo webHookSubscription)
        {
            return Task.CompletedTask;
        }

        public void Insert(WebHookSubscriptionInfo webHookSubscription)
        {
        }

        public Task UpdateAsync(WebHookSubscriptionInfo webHookSubscription)
        {
            return Task.CompletedTask;
        }

        public void Update(WebHookSubscriptionInfo webHookSubscription)
        {
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }

        public void Delete(Guid id)
        {
        }

        public Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId)
        {
            return Task.FromResult(new List<WebHookSubscriptionInfo>());
        }

        public List<WebHookSubscriptionInfo> GetAllSubscriptions(int? tenantId)
        {
            return new List<WebHookSubscriptionInfo>();
        }

        public Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(int? tenantId, string webHookName)
        {
            return Task.FromResult(new List<WebHookSubscriptionInfo>());
        }

        public List<WebHookSubscriptionInfo> GetAllSubscriptions(int? tenantId, string webHookName)
        {
            return new List<WebHookSubscriptionInfo>();
        }

        public Task<bool> IsSubscribedAsync(int? tenantId, string webHookName)
        {
            return Task.FromResult(false);
        }

        public bool IsSubscribed(int? tenantId, string webHookName)
        {
            return false;
        }
    }
}