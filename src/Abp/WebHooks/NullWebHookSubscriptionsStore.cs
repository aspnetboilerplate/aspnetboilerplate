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
            return Task.FromResult<WebHookSubscriptionInfo>(null);
        }

        public WebHookSubscriptionInfo Get(Guid id)
        {
            return null;
        }

        public Task InsertAsync(WebHookSubscriptionInfo webHookInfo)
        {
            return Task.CompletedTask;
        }

        public void Insert(WebHookSubscriptionInfo webHookInfo)
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

        public Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(string webHookDefinitionName)
        {
            return Task.FromResult(new List<WebHookSubscriptionInfo>());
        }

        public List<WebHookSubscriptionInfo> GetAllSubscriptions(string webHookDefinitionName)
        {
            return new List<WebHookSubscriptionInfo>();
        }

        public Task<bool> IsSubscribedAsync(UserIdentifier user, string webHookName)
        {
            return Task.FromResult<bool>(false);
        }

        public bool IsSubscribed(UserIdentifier user, string webHookName)
        {
            return false;
        }

        public Task<List<WebHookSubscriptionInfo>> GetSubscribedWebHooksAsync(UserIdentifier user)
        {
            return Task.FromResult(new List<WebHookSubscriptionInfo>());
        }

        public List<WebHookSubscriptionInfo> GetSubscribedWebHooks(UserIdentifier user)
        {
            return new List<WebHookSubscriptionInfo>();
        }

        public Task SetActiveAsync(Guid id, bool active)
        {
            return Task.CompletedTask;
        }

        public void SetActive(Guid id, bool active)
        {

        }
    }
}