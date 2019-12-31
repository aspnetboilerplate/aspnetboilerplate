using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Domain.Uow;

namespace Abp.WebHooks
{
    public class WebHookSubscriptionManager : DomainService, IWebHookSubscriptionManager
    {
        public IWebHookSubscriptionsStore WebHookSubscriptionsStore { get; set; }
        private readonly IGuidGenerator _guidGenerator;

        public WebHookSubscriptionManager(
            IGuidGenerator guidGenerator)
        {
            _guidGenerator = guidGenerator;

            WebHookSubscriptionsStore = NullWebHookSubscriptionsStore.Instance;
        }

        public async Task<WebHookSubscription> GetAsync(Guid id)
        {
            return (await WebHookSubscriptionsStore.GetAsync(id)).ToWebHookSubscription();
        }

        public WebHookSubscription Get(Guid id)
        {
            return WebHookSubscriptionsStore.Get(id).ToWebHookSubscription();
        }

        public async Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(string webHookName)
        {
            return (await WebHookSubscriptionsStore.GetAllSubscriptionsAsync(webHookName))
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public List<WebHookSubscription> GetAllSubscriptions(string webHookName)
        {
            return WebHookSubscriptionsStore.GetAllSubscriptions(webHookName)
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public Task<bool> IsSubscribedAsync(UserIdentifier user, string webHookName)
        {
            return WebHookSubscriptionsStore.IsSubscribedAsync(user, webHookName);
        }

        public bool IsSubscribed(UserIdentifier user, string webHookName)
        {
            return WebHookSubscriptionsStore.IsSubscribed(user, webHookName);
        }

        [UnitOfWork]
        public Task ActivateSubscriptionAsync(Guid id)
        {
            return WebHookSubscriptionsStore.SetActiveAsync(id, true);
        }

        [UnitOfWork]
        public void ActivateSubscription(Guid id)
        {
            WebHookSubscriptionsStore.SetActive(id, true);
        }

        [UnitOfWork]
        public Task DeactivateSubscriptionAsync(Guid id)
        {
            return WebHookSubscriptionsStore.SetActiveAsync(id, false);
        }

        [UnitOfWork]
        public void DeactivateSubscription(Guid id)
        {
            WebHookSubscriptionsStore.SetActive(id, false);
        }

        public Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.Id == default)
            {
                return WebHookSubscriptionsStore.InsertAsync(webHookSubscription.ToWebHookSubscriptionInfo());
            }

            return WebHookSubscriptionsStore.UpdateSubscriptionAsync(webHookSubscription.ToWebHookSubscriptionInfo());
        }

        public void AddOrUpdateSubscription(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.Id == default)
            {
                webHookSubscription.Id = _guidGenerator.Create();
                WebHookSubscriptionsStore.Insert(webHookSubscription.ToWebHookSubscriptionInfo());
            }
            else
            {
                WebHookSubscriptionsStore.UpdateSubscription(webHookSubscription.ToWebHookSubscriptionInfo());
            }
        }

        [UnitOfWork]
        public async Task AddWebHookAsync(Guid id, string webHookName)
        {
            var subscription = await WebHookSubscriptionsStore.GetAsync(id);
            subscription.AddWebHookDefinition(webHookName);
        }

        [UnitOfWork]
        public void AddWebHook(Guid id, string webHookName)
        {
            var subscription = WebHookSubscriptionsStore.Get(id);
            subscription.AddWebHookDefinition(webHookName);
        }

        public async Task<List<WebHookSubscription>> GetSubscribedWebHooksAsync(UserIdentifier user)
        {
            return (await WebHookSubscriptionsStore.GetSubscribedWebHooksAsync(user))
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public List<WebHookSubscription> GetSubscribedWebHooks(UserIdentifier user)
        {
            return WebHookSubscriptionsStore.GetSubscribedWebHooks(user)
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }
    }
}
