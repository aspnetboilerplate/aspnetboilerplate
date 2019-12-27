using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace Abp.WebHooks
{
    public class WebHookSubscriptionManager : IWebHookSubscriptionManager
    {
        private readonly IWebHookSubscriptionsStore _webHookSubscriptionsStore;
        private readonly IGuidGenerator _guidGenerator;

        public WebHookSubscriptionManager(
            IWebHookSubscriptionsStore webHookSubscriptionsStore,
            IGuidGenerator guidGenerator)
        {
            _webHookSubscriptionsStore = webHookSubscriptionsStore;
            _guidGenerator = guidGenerator;
        }

        public async Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(string webHookName)
        {
            return (await _webHookSubscriptionsStore.GetAllSubscriptionsAsync(webHookName))
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public List<WebHookSubscription> GetAllSubscriptions(string webHookName)
        {
            return _webHookSubscriptionsStore.GetAllSubscriptions(webHookName)
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public Task<bool> IsSubscribedAsync(UserIdentifier user, string webHookName)
        {
            return _webHookSubscriptionsStore.IsSubscribedAsync(user, webHookName);
        }

        public bool IsSubscribed(UserIdentifier user, string webHookName)
        {
            return _webHookSubscriptionsStore.IsSubscribed(user, webHookName);
        }

        [UnitOfWork]
        public Task ActivateSubscriptionAsync(Guid id)
        {
            return _webHookSubscriptionsStore.SetActiveAsync(id, true);
        }

        [UnitOfWork]
        public void ActivateSubscription(Guid id)
        {
            _webHookSubscriptionsStore.SetActive(id, true);
        }

        [UnitOfWork]
        public Task DeactivateSubscriptionAsync(Guid id)
        {
            return _webHookSubscriptionsStore.SetActiveAsync(id, false);
        }

        [UnitOfWork]
        public void DeactivateSubscription(Guid id)
        {
            _webHookSubscriptionsStore.SetActive(id, false);
        }

        public Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.Id == default)
            {
                return _webHookSubscriptionsStore.InsertAsync(webHookSubscription.ToWebHookSubscriptionInfo());
            }

            return _webHookSubscriptionsStore.UpdateSubscriptionAsync(webHookSubscription.ToWebHookSubscriptionInfo());
        }

        public void AddOrUpdateSubscription(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.Id == default)
            {
                webHookSubscription.Id = _guidGenerator.Create();
                _webHookSubscriptionsStore.Insert(webHookSubscription.ToWebHookSubscriptionInfo());
            }
            else
            {
                _webHookSubscriptionsStore.UpdateSubscription(webHookSubscription.ToWebHookSubscriptionInfo());
            }
        }

        [UnitOfWork]
        public async Task AddWebHookAsync(Guid id, string webHookName)
        {
            var subscription = await _webHookSubscriptionsStore.GetAsync(id);
            subscription.AddWebHookDefinition(webHookName);
        }

        [UnitOfWork]
        public void AddWebHook(Guid id, string webHookName)
        {
            var subscription = _webHookSubscriptionsStore.Get(id);
            subscription.AddWebHookDefinition(webHookName);
        }

        public async Task<List<WebHookSubscription>> GetSubscribedWebHooksAsync(UserIdentifier user)
        {
            return (await _webHookSubscriptionsStore.GetSubscribedWebHooksAsync(user))
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public List<WebHookSubscription> GetSubscribedWebHooks(UserIdentifier user)
        {
            return _webHookSubscriptionsStore.GetSubscribedWebHooks(user)
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }
    }
}
