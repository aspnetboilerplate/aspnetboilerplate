using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Domain.Services;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.WebHooks.BackgroundWorker;

namespace Abp.WebHooks
{
    public class DefaultWebHookPublisher : DomainService, IWebHookPublisher
    {
        public IAbpSession AbpSession { get; set; }
        public IWebHookStore WebHookStore { get; set; }

        private readonly IGuidGenerator _guidGenerator;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IWebHookSubscriptionManager _webHookSubscriptionManager;
        private readonly IWebHooksConfiguration _webHooksConfiguration;

        public DefaultWebHookPublisher(
            IWebHookSubscriptionManager webHookSubscriptionManager,
            IWebHooksConfiguration webHooksConfiguration,
            IGuidGenerator guidGenerator,
            IBackgroundJobManager backgroundJobManager)
        {
            _guidGenerator = guidGenerator;
            _backgroundJobManager = backgroundJobManager;
            _webHookSubscriptionManager = webHookSubscriptionManager;
            _webHooksConfiguration = webHooksConfiguration;

            WebHookStore = NullWebHookStore.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public async Task PublishAsync(string webHookName, object data)
        {
            var subscriptions = await _webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(AbpSession.TenantId, webHookName);
            await PublishAsync(AbpSession.TenantId, webHookName, data, subscriptions);
        }

        public async Task PublishAsync(string webHookName, object data, int? tenantId)
        {
            var subscriptions = await _webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(tenantId, webHookName);
            await PublishAsync(tenantId, webHookName, data, subscriptions);
        }

        private async Task PublishAsync(int? tenantId, string webHookName, object data, List<WebHookSubscription> webHookSubscriptions)
        {
            if (webHookSubscriptions == null || webHookSubscriptions.Count == 0)
            {
                return;
            }

            var webHookInfo = await SaveAndGetWebHookAsync(tenantId, webHookName, data);

            foreach (var webHookSubscription in webHookSubscriptions)
            {
                await _backgroundJobManager.EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(new WebHookSenderInput()
                {
                    TenantId = webHookSubscription.TenantId,
                    WebHookId = webHookInfo.Id,
                    Data = webHookInfo.Data,
                    WebHookDefinition = webHookInfo.WebHookDefinition,

                    WebHookSubscriptionId = webHookSubscription.Id,
                    Headers = webHookSubscription.Headers,
                    Secret = webHookSubscription.Secret,
                    WebHookUri = webHookSubscription.WebHookUri
                });
            }
        }

        public void Publish(string webHookName, object data)
        {
            var subscriptions = _webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(AbpSession.TenantId, webHookName);
            Publish(AbpSession.TenantId, webHookName, data, subscriptions);
        }

        public void Publish(string webHookName, object data, int? tenantId)
        {
            var subscriptions = _webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGranted(tenantId, webHookName);
            Publish(tenantId, webHookName, data, subscriptions);
        }

        private void Publish(int? tenantId, string webHookName, object data, List<WebHookSubscription> webHookSubscriptions)
        {
            if (webHookSubscriptions == null || webHookSubscriptions.Count == 0)
            {
                return;
            }

            var webHookInfo = SaveAndGetWebHook(tenantId, webHookName, data);

            foreach (var webHookSubscription in webHookSubscriptions)
            {
                _backgroundJobManager.Enqueue<WebHookSenderJob, WebHookSenderInput>(new WebHookSenderInput()
                {
                    TenantId = webHookSubscription.TenantId,
                    WebHookId = webHookInfo.Id,
                    Data = webHookInfo.Data,
                    WebHookDefinition = webHookInfo.WebHookDefinition,

                    WebHookSubscriptionId = webHookSubscription.Id,
                    Headers = webHookSubscription.Headers,
                    Secret = webHookSubscription.Secret,
                    WebHookUri = webHookSubscription.WebHookUri
                });
            }
        }

        protected virtual async Task<WebHookInfo> SaveAndGetWebHookAsync(int? tenantId, string webHookName, object data)
        {
            var webHookInfo = new WebHookInfo()
            {
                Id = _guidGenerator.Create(),
                WebHookDefinition = webHookName,
                Data = _webHooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString(),
                TenantId = tenantId
            };

            await WebHookStore.InsertAndGetIdAsync(webHookInfo);
            return webHookInfo;
        }

        protected virtual WebHookInfo SaveAndGetWebHook(int? tenantId, string webHookName, object data)
        {
            var webHookInfo = new WebHookInfo()
            {
                Id = _guidGenerator.Create(),
                WebHookDefinition = webHookName,
                Data = _webHooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString(),
                TenantId = tenantId
            };

            WebHookStore.InsertAndGetId(webHookInfo);
            return webHookInfo;
        }
    }
}
