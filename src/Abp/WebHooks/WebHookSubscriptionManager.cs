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

        public async Task<List<WebHookSubscription>> GetAllSubscriptionsPermissionGrantedAsync(UserIdentifier user, string webHookName)
        {
            if (!await _webHookDefinitionManager.IsAvailableAsync(user, webHookName))
            {
                return new List<WebHookSubscription>();
            }

            return (await WebHookSubscriptionsStore.GetAllSubscriptionsAsync(user, webHookName))
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public List<WebHookSubscription> GetAllSubscriptionsPermissionGranted(UserIdentifier user, string webHookName)
        {
            if (!_webHookDefinitionManager.IsAvailable(user, webHookName))
            {
                return new List<WebHookSubscription>();
            }

            return WebHookSubscriptionsStore.GetAllSubscriptions(user)
                .Select(x => x.ToWebHookSubscription())
                .ToList();
        }

        public async Task<List<WebHookSubscription>> GetAllSubscriptionsPermissionGrantedAsync(string webHookName)
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

        public List<WebHookSubscription> GetAllSubscriptionsPermissionGranted(string webHookName)
        {
            return WebHookSubscriptionsStore.GetAllSubscriptions(webHookName)
                .Select(x => x.ToWebHookSubscription())
                .Where(x => _webHookDefinitionManager.IsAvailable(new UserIdentifier(x.TenantId, x.UserId), webHookName))
                .ToList();
        }

        public async Task<bool> IsSubscribedAsync(UserIdentifier user, string webHookName)
        {
            if (!await _webHookDefinitionManager.IsAvailableAsync(user, webHookName))
            {
                return false;
            }

            return await WebHookSubscriptionsStore.IsSubscribedAsync(user, webHookName);
        }

        public bool IsSubscribed(UserIdentifier user, string webHookName)
        {
            if (!_webHookDefinitionManager.IsAvailable(user, webHookName))
            {
                return false;
            }

            return WebHookSubscriptionsStore.IsSubscribed(user, webHookName);
        }

        public Task ActivateSubscriptionAsync(Guid id)
        {
            return WebHookSubscriptionsStore.SetActiveAsync(id, true);
        }

        public void ActivateSubscription(Guid id)
        {
            WebHookSubscriptionsStore.SetActive(id, true);
        }

        public Task DeactivateSubscriptionAsync(Guid id)
        {
            return WebHookSubscriptionsStore.SetActiveAsync(id, false);
        }

        public void DeactivateSubscription(Guid id)
        {
            WebHookSubscriptionsStore.SetActive(id, false);
        }

        public async Task AddOrUpdateSubscriptionAsync(WebHookSubscription webHookSubscription)
        {
            await CheckIfPermissionsGrantedAsync(webHookSubscription);

            if (webHookSubscription.Id == default)
            {
                webHookSubscription.Id = _guidGenerator.Create();
                await WebHookSubscriptionsStore.InsertAsync(webHookSubscription.ToWebHookSubscriptionInfo());
            }
            else
            {
                await WebHookSubscriptionsStore.UpdateAsync(webHookSubscription.ToWebHookSubscriptionInfo());
            }
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

            await CheckPermissionsAsync(new UserIdentifier(subscription.TenantId, subscription.UserId), webHookName);

            subscription.AddWebHookDefinition(webHookName);
        }

        [UnitOfWork]
        public void AddWebHook(Guid id, string webHookName)
        {
            var subscription = WebHookSubscriptionsStore.Get(id);

            CheckPermissions(new UserIdentifier(subscription.TenantId, subscription.UserId), webHookName);

            subscription.AddWebHookDefinition(webHookName);
        }

        #region PermissionCheck
        private void CheckIfPermissionsGranted(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.WebHookDefinitions == null || webHookSubscription.WebHookDefinitions.Count == 0)
            {
                return;
            }

            var userIdentifier = new UserIdentifier(webHookSubscription.TenantId, webHookSubscription.UserId);

            foreach (var webHookDefinition in webHookSubscription.WebHookDefinitions)
            {
                CheckPermissions(userIdentifier, webHookDefinition);
            }
        }

        private async Task CheckIfPermissionsGrantedAsync(WebHookSubscription webHookSubscription)
        {
            if (webHookSubscription.WebHookDefinitions == null || webHookSubscription.WebHookDefinitions.Count == 0)
            {
                return;
            }

            var userIdentifier = new UserIdentifier(webHookSubscription.TenantId, webHookSubscription.UserId);

            foreach (var webHookDefinition in webHookSubscription.WebHookDefinitions)
            {
                await CheckPermissionsAsync(userIdentifier, webHookDefinition);
            }
        }

        private void CheckPermissions(UserIdentifier userIdentifier, string webHookName)
        {
            if (!_webHookDefinitionManager.IsAvailable(userIdentifier, webHookName))
            {
                //TODO: improve exception message
                throw new AbpAuthorizationException($"Necessary permissions to have \"{webHookName}\" are not granted to user \"{userIdentifier.ToUserIdentifierString()}\"");
            }
        }

        private async Task CheckPermissionsAsync(UserIdentifier userIdentifier, string webHookName)
        {
            if (!await _webHookDefinitionManager.IsAvailableAsync(userIdentifier, webHookName))
            {
                //TODO: improve exception message
                throw new AbpAuthorizationException($"Necessary permissions to have \"{webHookName}\" are not granted to user \"{userIdentifier.ToUserIdentifierString()}\"");
            }
        }
        #endregion
    }
}
