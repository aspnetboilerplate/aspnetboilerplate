using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.Webhooks.Extensions;

namespace Abp.Webhooks
{
    public class WebhookSubscriptionManager : DomainService, IWebhookSubscriptionManager
    {
        public IWebhookSubscriptionsStore WebhookSubscriptionsStore { get; set; }

        private readonly IGuidGenerator _guidGenerator;

        private readonly IWebhookDefinitionManager _webhookDefinitionManager;

        private const string WebhookSubscriptionSecretPrefix = "whs_";

        public WebhookSubscriptionManager(
            IGuidGenerator guidGenerator,
            IWebhookDefinitionManager webhookDefinitionManager)
        {
            _guidGenerator = guidGenerator;
            _webhookDefinitionManager = webhookDefinitionManager;

            WebhookSubscriptionsStore = NullWebhookSubscriptionsStore.Instance;
        }

        public async Task<WebhookSubscription> GetAsync(Guid id)
        {
            return (await WebhookSubscriptionsStore.GetAsync(id)).ToWebhookSubscription();
        }

        public WebhookSubscription Get(Guid id)
        {
            return WebhookSubscriptionsStore.Get(id).ToWebhookSubscription();
        }

        public async Task<List<WebhookSubscription>> GetAllSubscriptionsAsync(int? tenantId)
        {
            return (await WebhookSubscriptionsStore.GetAllSubscriptionsAsync(tenantId))
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public List<WebhookSubscription> GetAllSubscriptions(int? tenantId)
        {
            return WebhookSubscriptionsStore.GetAllSubscriptions(tenantId)
            .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public async Task<List<WebhookSubscription>> GetAllSubscriptionsIfFeaturesGrantedAsync(int? tenantId, string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(tenantId, webhookName))
            {
                return new List<WebhookSubscription>();
            }

            return (await WebhookSubscriptionsStore.GetAllSubscriptionsAsync(tenantId, webhookName))
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public List<WebhookSubscription> GetAllSubscriptionsIfFeaturesGranted(int? tenantId, string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(tenantId, webhookName))
            {
                return new List<WebhookSubscription>();
            }

            return WebhookSubscriptionsStore.GetAllSubscriptions(tenantId, webhookName)
                .Select(subscriptionInfo => subscriptionInfo.ToWebhookSubscription()).ToList();
        }

        public async Task<bool> IsSubscribedAsync(int? tenantId, string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(tenantId, webhookName))
            {
                return false;
            }

            return await WebhookSubscriptionsStore.IsSubscribedAsync(tenantId, webhookName);
        }

        public bool IsSubscribed(int? tenantId, string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(tenantId, webhookName))
            {
                return false;
            }

            return WebhookSubscriptionsStore.IsSubscribed(tenantId, webhookName);
        }

        [UnitOfWork]
        public async Task AddOrUpdateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            await CheckIfPermissionsGrantedAsync(webhookSubscription);

            if (webhookSubscription.Id == default)
            {
                webhookSubscription.Id = _guidGenerator.Create();
                webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString().Replace("-", "");
                await WebhookSubscriptionsStore.InsertAsync(webhookSubscription.ToWebhookSubscriptionInfo());
            }
            else
            {
                var subscription = await WebhookSubscriptionsStore.GetAsync(webhookSubscription.Id);
                subscription.WebhookUri = webhookSubscription.WebhookUri;
                subscription.Webhooks = webhookSubscription.Webhooks.ToJsonString();
                subscription.Headers = webhookSubscription.Headers.ToJsonString();
                await WebhookSubscriptionsStore.UpdateAsync(subscription);
            }
        }

        [UnitOfWork]
        public void AddOrUpdateSubscription(WebhookSubscription webhookSubscription)
        {
            CheckIfPermissionsGranted(webhookSubscription);

            if (webhookSubscription.Id == default)
            {
                webhookSubscription.Id = _guidGenerator.Create();
                webhookSubscription.Secret = WebhookSubscriptionSecretPrefix + Guid.NewGuid().ToString().Replace("-", "");
                WebhookSubscriptionsStore.Insert(webhookSubscription.ToWebhookSubscriptionInfo());
            }
            else
            {
                var subscription = WebhookSubscriptionsStore.Get(webhookSubscription.Id);
                subscription.WebhookUri = webhookSubscription.WebhookUri;
                subscription.Webhooks = webhookSubscription.Webhooks.ToJsonString();
                subscription.Headers = webhookSubscription.Headers.ToJsonString();
                WebhookSubscriptionsStore.Update(subscription);
            }
        }

        [UnitOfWork]
        public async Task ActivateWebhookSubscriptionAsync(Guid id, bool active)
        {
            var webhookSubscription = await WebhookSubscriptionsStore.GetAsync(id);
            webhookSubscription.IsActive = active;
        }

        [UnitOfWork]
        public void ActivateWebhookSubscription(Guid id, bool active)
        {
            var webhookSubscription = WebhookSubscriptionsStore.Get(id);
            webhookSubscription.IsActive = active;
        }

        [UnitOfWork]
        public async Task AddWebhookAsync(WebhookSubscriptionInfo subscription, string webhookName)
        {
            await CheckPermissionsAsync(subscription.TenantId, webhookName);

            subscription.SubscribeWebhook(webhookName);
        }

        [UnitOfWork]
        public void AddWebhook(WebhookSubscriptionInfo subscription, string webhookName)
        {
            CheckPermissions(subscription.TenantId, webhookName);

            subscription.SubscribeWebhook(webhookName);
        }

        #region PermissionCheck

        private async Task CheckIfPermissionsGrantedAsync(WebhookSubscription webhookSubscription)
        {
            if (webhookSubscription.Webhooks.IsNullOrEmpty())
            {
                return;
            }

            foreach (var webhookDefinition in webhookSubscription.Webhooks)
            {
                await CheckPermissionsAsync(webhookSubscription.TenantId, webhookDefinition);
            }
        }

        private async Task CheckPermissionsAsync(int? tenantId, string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(tenantId, webhookName))
            {
                throw new AbpAuthorizationException($"Tenant \"{tenantId}\" must have necessary feature(s) to use webhook \"{webhookName}\"");
            }
        }

        private void CheckIfPermissionsGranted(WebhookSubscription webhookSubscription)
        {
            if (webhookSubscription.Webhooks.IsNullOrEmpty())
            {
                return;
            }

            foreach (var webhookDefinition in webhookSubscription.Webhooks)
            {
                CheckPermissions(webhookSubscription.TenantId, webhookDefinition);
            }
        }

        private void CheckPermissions(int? tenantId, string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(tenantId, webhookName))
            {
                throw new AbpAuthorizationException($"Tenant \"{tenantId}\" must have necessary feature(s) to use webhook \"{webhookName}\"");
            }
        }


        #endregion
    }
}
