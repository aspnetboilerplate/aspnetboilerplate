using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Services;
using Abp.Domain.Uow;

namespace Abp.WebHooks
{
    public class WebHookSubscriptionManager : DomainService, IWebHookSubscriptionManager
    {
        public IWebHookSubscriptionsStore WebHookSubscriptionsStore { get; set; }
        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebHookDefinitionManager _webHookDefinitionManager;

        public WebHookSubscriptionManager(
            IGuidGenerator guidGenerator,
            IWebHookDefinitionManager webHookDefinitionManager)
        {
            _guidGenerator = guidGenerator;
            _webHookDefinitionManager = webHookDefinitionManager;

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
            var allList = await WebHookSubscriptionsStore.GetAllSubscriptionsAsync(webHookName);

            var permissionGrantedList = new List<WebHookSubscription>();

            foreach (var webHookSubscriptionInfo in allList)
            {
                if (await _webHookDefinitionManager.IsAvailableAsync(new UserIdentifier(webHookSubscriptionInfo.TenantId, webHookSubscriptionInfo.UserId), webHookName))
                {
                    permissionGrantedList.Add(webHookSubscriptionInfo.ToWebHookSubscription());
                }
            }

            return permissionGrantedList;
        }

        public List<WebHookSubscription> GetAllSubscriptions(string webHookName)
        {
            return WebHookSubscriptionsStore.GetAllSubscriptions(webHookName)
                .Select(x => x.ToWebHookSubscription())
                .Where(x => _webHookDefinitionManager.IsAvailable(new UserIdentifier(x.TenantId, x.UserId), webHookName))
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

        public async Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription)
        {
            await CheckIfPermissionsGrantedAsync(webHookSubscription);

            if (webHookSubscription.Id == default)
            {
                await WebHookSubscriptionsStore.InsertAsync(webHookSubscription.ToWebHookSubscriptionInfo());
            }

            await WebHookSubscriptionsStore.UpdateAsync(webHookSubscription.ToWebHookSubscriptionInfo());
        }

        public void AddOrUpdateSubscription(WebHookSubscription webHookSubscription)
        {
            CheckIfPermissionsGranted(webHookSubscription);

            if (webHookSubscription.Id == default)
            {
                webHookSubscription.Id = _guidGenerator.Create();
                WebHookSubscriptionsStore.Insert(webHookSubscription.ToWebHookSubscriptionInfo());
            }
            else
            {
                WebHookSubscriptionsStore.Update(webHookSubscription.ToWebHookSubscriptionInfo());
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

        public async Task<List<WebHookSubscription>> GetAllSubscriptionsAsync(UserIdentifier user)
        {
            return (await WebHookSubscriptionsStore.GetAllSubscriptionsAsync(user))
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public List<WebHookSubscription> GetAllSubscriptions(UserIdentifier user)
        {
            return WebHookSubscriptionsStore.GetAllSubscriptions(user)
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        protected virtual void CheckIfPermissionsGranted(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.WebHookDefinitions == null || webHookSubscription.WebHookDefinitions.Count == 0)
            {
                return;
            }

            var userIdentifier = new UserIdentifier(webHookSubscription.TenantId, webHookSubscription.UserId);

            foreach (var webHookDefinition in webHookSubscription.WebHookDefinitions)
            {
                if (!_webHookDefinitionManager.IsAvailable(userIdentifier, webHookDefinition))
                {
                    //TODO: improve exception message
                    throw new AbpAuthorizationException($"User {userIdentifier.ToUserIdentifierString()} is not granted for webhook \"{webHookDefinition}\"");
                }
            }
        }

        protected virtual async Task CheckIfPermissionsGrantedAsync(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.WebHookDefinitions == null || webHookSubscription.WebHookDefinitions.Count == 0)
            {
                return;
            }

            var userIdentifier = new UserIdentifier(webHookSubscription.TenantId, webHookSubscription.UserId);

            foreach (var webHookDefinition in webHookSubscription.WebHookDefinitions)
            {
                if (!await _webHookDefinitionManager.IsAvailableAsync(userIdentifier, webHookDefinition))
                {
                    //TODO: improve exception message
                    throw new AbpAuthorizationException($"User {userIdentifier.ToUserIdentifierString()} is not granted for webhook \"{webHookDefinition}\"");
                }
            }
        }
    }
}
