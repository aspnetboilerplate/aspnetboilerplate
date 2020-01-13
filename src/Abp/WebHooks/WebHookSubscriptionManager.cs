using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Services;
using Abp.Domain.Uow;

namespace Abp.Webhooks
{
    public class WebhookSubscriptionManager : DomainService, IWebhookSubscriptionManager
    {
        public IWebhookSubscriptionsStore WebhookSubscriptionsStore { get; set; }

        private readonly IGuidGenerator _guidGenerator;

        private readonly IWebhookDefinitionManager _webhookDefinitionManager;

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
                .Select(x => x.ToWebhookSubscription()).ToList();
        }

        public List<WebhookSubscription> GetAllSubscriptions(int? tenantId)
        {
            return WebhookSubscriptionsStore.GetAllSubscriptions(tenantId)
            .Select(x => x.ToWebhookSubscription()).ToList();
        }

        public async Task<List<WebhookSubscription>> GetAllSubscriptionsIfFeaturesGrantedAsync(int? tenantId, string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(tenantId, webhookName))
            {
                return new List<WebhookSubscription>();
            }

            return (await WebhookSubscriptionsStore.GetAllSubscriptionsAsync(tenantId, webhookName))
                .Select(x => x.ToWebhookSubscription()).ToList();
        }

        public List<WebhookSubscription> GetAllSubscriptionsIfFeaturesGranted(int? tenantId, string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(tenantId, webhookName))
            {
                return new List<WebhookSubscription>();
            }

            return WebhookSubscriptionsStore.GetAllSubscriptions(tenantId, webhookName)
                .Select(x => x.ToWebhookSubscription()).ToList();
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

        public async Task AddOrUpdateSubscriptionAsync(WebhookSubscription webhookSubscription)
        {
            await CheckIfPermissionsGrantedAsync(webhookSubscription);

            if (webhookSubscription.Id == default)
            {
                webhookSubscription.Id = _guidGenerator.Create();
                webhookSubscription.Secret = "whs_" + Guid.NewGuid().ToString().Replace("-", "");
                await WebhookSubscriptionsStore.InsertAsync(webhookSubscription.ToWebhookSubscriptionInfo());
            }
            else
            {
                await WebhookSubscriptionsStore.UpdateAsync(webhookSubscription.ToWebhookSubscriptionInfo());
            }
        }

        public void AddOrUpdateSubscription(WebhookSubscription webhookSubscription)
        {
            CheckIfPermissionsGranted(webhookSubscription);

            if (webhookSubscription.Id == default)
            {
                webhookSubscription.Id = _guidGenerator.Create();
                webhookSubscription.Secret = "whs_" + Guid.NewGuid().ToString().Replace("-", "");
                WebhookSubscriptionsStore.Insert(webhookSubscription.ToWebhookSubscriptionInfo());
            }
            else
            {
                WebhookSubscriptionsStore.Update(webhookSubscription.ToWebhookSubscriptionInfo());
            }
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
            if (webhookSubscription.WebhookDefinitions == null || webhookSubscription.WebhookDefinitions.Count == 0)
            {
                return;
            }

            foreach (var webhookDefinition in webhookSubscription.WebhookDefinitions)
            {
                await CheckPermissionsAsync(webhookSubscription.TenantId, webhookDefinition);
            }
        }

        private async Task CheckPermissionsAsync(int? tenantId, string webhookName)
        {
            if (!await _webhookDefinitionManager.IsAvailableAsync(tenantId, webhookName))
            {
                //TODO: improve exception message
                throw new AbpAuthorizationException($"Tenant \"{tenantId}\" must have necessary feature(s) to use webhook \"{webhookName}\"");
            }
        }

        private void CheckIfPermissionsGranted(WebhookSubscription webhookSubscription)
        {
            if (webhookSubscription.WebhookDefinitions == null || webhookSubscription.WebhookDefinitions.Count == 0)
            {
                return;
            }

            foreach (var webhookDefinition in webhookSubscription.WebhookDefinitions)
            {
                CheckPermissions(webhookSubscription.TenantId, webhookDefinition);
            }
        }

        private void CheckPermissions(int? tenantId, string webhookName)
        {
            if (!_webhookDefinitionManager.IsAvailable(tenantId, webhookName))
            {
                //TODO: improve exception message
                throw new AbpAuthorizationException($"Tenant \"{tenantId}\" must have necessary feature(s) to use webhook \"{webhookName}\"");
            }
        }


        #endregion
    }
}
