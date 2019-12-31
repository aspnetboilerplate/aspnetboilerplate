using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.WebHooks.BackgroundWorker;

namespace Abp.WebHooks
{
    public class DefaultWebHookPublisher : AbpServiceBase, IWebHookPublisher, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IWebHookSubscriptionManager _webHookSubscriptionManager;
        private readonly IWebHooksConfiguration _webHooksConfiguration;

        public IWebHookStore WebHookStore { get; set; }

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
        }

        [UnitOfWork]
        public virtual async Task PublishAsync(string webHookName, object data)
        {
            var webHookId = await SaveWebHookAndGetIdAsync(webHookName, data);

            var subscriptions = await _webHookSubscriptionManager.GetAllSubscriptionsAsync(webHookName);

            foreach (var webHookSubscription in subscriptions)
            {
                await _backgroundJobManager.EnqueueAsync<WebHookSenderJob, WebHookSenderJobArgs>(new WebHookSenderJobArgs()
                {
                    WebHookId = webHookId,
                    WebHookSubscriptionId = webHookSubscription.Id
                });
            }
        }

        [UnitOfWork]
        public virtual void Publish(string webHookName, object data)
        {
            var webHookId = SaveWebHookAndGetId(webHookName, data);

            var subscriptions = _webHookSubscriptionManager.GetAllSubscriptions(webHookName);

            foreach (var webHookSubscription in subscriptions)
            {
                _backgroundJobManager.Enqueue<WebHookSenderJob, WebHookSenderJobArgs>(new WebHookSenderJobArgs()
                {
                    WebHookId = webHookId,
                    WebHookSubscriptionId = webHookSubscription.Id
                });
            }
        }

        protected virtual Task<Guid> SaveWebHookAndGetIdAsync(string webHookName, object data)
        {
            var webHookInfo = new WebHookInfo()
            {
                Id = _guidGenerator.Create(),
                WebHookDefinition = webHookName,
                Data = _webHooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString()
            };

            return WebHookStore.InsertAndGetIdAsync(webHookInfo);
        }

        protected virtual Guid SaveWebHookAndGetId(string webHookName, object data)
        {
            var webHookInfo = new WebHookInfo()
            {
                Id = _guidGenerator.Create(),
                WebHookDefinition = webHookName,
                Data = _webHooksConfiguration.JsonSerializerSettings != null
                    ? data.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                    : data.ToJsonString()
            };

            return WebHookStore.InsertAndGetId(webHookInfo);
        }
    }
}
