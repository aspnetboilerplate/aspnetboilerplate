using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.BackgroundJobs;
using Abp.Json;
using Abp.Webhooks.BackgroundWorker;
using Abp.Collections.Extensions;

namespace Abp.Webhooks
{
    public class DefaultWebhookPublisher : ApplicationService, IWebhookPublisher
    {
        public IWebhookEventStore WebhookEventStore { get; set; }

        private readonly IGuidGenerator _guidGenerator;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;
        private readonly IWebhooksConfiguration _webhooksConfiguration;

        public DefaultWebhookPublisher(
            IWebhookSubscriptionManager webhookSubscriptionManager,
            IWebhooksConfiguration webhooksConfiguration,
            IGuidGenerator guidGenerator,
            IBackgroundJobManager backgroundJobManager)
        {
            _guidGenerator = guidGenerator;
            _backgroundJobManager = backgroundJobManager;
            _webhookSubscriptionManager = webhookSubscriptionManager;
            _webhooksConfiguration = webhooksConfiguration;

            WebhookEventStore = NullWebhookEventStore.Instance;
        }

        public async Task PublishAsync(string webhookName, object data, bool sendExactSameData = false)
        {
            var subscriptions = await _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(AbpSession.TenantId, webhookName);
            await PublishAsync(AbpSession.TenantId, webhookName, data, subscriptions, sendExactSameData);
        }

        public async Task PublishAsync(string webhookName, object data, int? tenantId, bool sendExactSameData = false)
        {
            var subscriptions = await _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, webhookName);
            await PublishAsync(tenantId, webhookName, data, subscriptions, sendExactSameData);
        }

        private async Task PublishAsync(int? tenantId, string webhookName, object data, List<WebhookSubscription> webhookSubscriptions, bool sendExactSameData = false)
        {
            if (webhookSubscriptions.IsNullOrEmpty())
            {
                return;
            }

            var webhookInfo = await SaveAndGetWebhookAsync(tenantId, webhookName, data);

            foreach (var webhookSubscription in webhookSubscriptions)
            {
                await _backgroundJobManager.EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(new WebhookSenderArgs
                {
                    TenantId = webhookSubscription.TenantId,
                    WebhookEventId = webhookInfo.Id,
                    Data = webhookInfo.Data,
                    WebhookName = webhookInfo.WebhookName,
                    WebhookSubscriptionId = webhookSubscription.Id,
                    Headers = webhookSubscription.Headers,
                    Secret = webhookSubscription.Secret,
                    WebhookUri = webhookSubscription.WebhookUri,
                    SendExactSameData = sendExactSameData
                });
            }
        }

        public void Publish(string webhookName, object data, bool sendExactSameData = false)
        {
            var subscriptions = _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(AbpSession.TenantId, webhookName);
            Publish(AbpSession.TenantId, webhookName, data, subscriptions, sendExactSameData);
        }

        public void Publish(string webhookName, object data, int? tenantId, bool sendExactSameData = false)
        {
            var subscriptions = _webhookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, webhookName);
            Publish(tenantId, webhookName, data, subscriptions, sendExactSameData);
        }

        private void Publish(int? tenantId, string webhookName, object data, List<WebhookSubscription> webhookSubscriptions, bool sendExactSameData = false)
        {
            if (webhookSubscriptions.IsNullOrEmpty())
            {
                return;
            }

            var webhookInfo = SaveAndGetWebhook(tenantId, webhookName, data);

            foreach (var webhookSubscription in webhookSubscriptions)
            {
                _backgroundJobManager.Enqueue<WebhookSenderJob, WebhookSenderArgs>(new WebhookSenderArgs
                {
                    TenantId = webhookSubscription.TenantId,
                    WebhookEventId = webhookInfo.Id,
                    Data = webhookInfo.Data,
                    WebhookName = webhookInfo.WebhookName,
                    WebhookSubscriptionId = webhookSubscription.Id,
                    Headers = webhookSubscription.Headers,
                    Secret = webhookSubscription.Secret,
                    WebhookUri = webhookSubscription.WebhookUri,
                    SendExactSameData = sendExactSameData
                });
            }
        }

        protected virtual async Task<WebhookEvent> SaveAndGetWebhookAsync(int? tenantId, string webhookName, object data)
        {
            var webhookInfo = new WebhookEvent
            {
                Id = _guidGenerator.Create(),
                WebhookName = webhookName,
                Data = _webhooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString(),
                TenantId = tenantId
            };

            await WebhookEventStore.InsertAndGetIdAsync(webhookInfo);
            return webhookInfo;
        }

        protected virtual WebhookEvent SaveAndGetWebhook(int? tenantId, string webhookName, object data)
        {
            var webhookInfo = new WebhookEvent
            {
                Id = _guidGenerator.Create(),
                WebhookName = webhookName,
                Data = _webhooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString(),
                TenantId = tenantId
            };

            WebhookEventStore.InsertAndGetId(webhookInfo);
            return webhookInfo;
        }
    }
}
