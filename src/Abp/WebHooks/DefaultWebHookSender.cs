using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.Threading;

namespace Abp.WebHooks
{
    public class DefaultWebHookSender : DomainService, IWebHookSender
    {
        internal const string SignatureHeaderKey = "sha256";
        internal const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";
        internal const string SignatureHeaderName = "abp-webhook-signature";

        private readonly IWebHookSubscriptionManager _webHookSubscriptionManager;
        private readonly IWebhookStoreManager _webhookStoreManager;
        private readonly IWebHooksConfiguration _webHooksConfiguration;


        public IWebHookWorkItemStore WebHookWorkItemStore { get; set; }

        public DefaultWebHookSender(
            IWebHookSubscriptionManager webHookSubscriptionManager,
            IWebhookStoreManager webhookStoreManager,
            IWebHooksConfiguration webHooksConfiguration)
        {
            _webHookSubscriptionManager = webHookSubscriptionManager;
            _webhookStoreManager = webhookStoreManager;
            _webHooksConfiguration = webHooksConfiguration;

            WebHookWorkItemStore = NullWebHookWorkItemStore.Instance;
        }

        public async Task<bool> TrySendWebHookAsync(Guid webHookId, Guid webHookSubscriptionId)
        {
            if (webHookId == default)
            {
                throw new ArgumentNullException(nameof(webHookId));
            }

            if (webHookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(webHookSubscriptionId));
            }

            var webhook = await _webhookStoreManager.GetAsync(webHookId);
            if (webhook == null)
            {
                throw new Exception("DefaultWebHookSender can not send webhook since could not found webhook by id: " + webHookId);
            }

            var subscription = await _webHookSubscriptionManager.GetAsync(webHookSubscriptionId);
            if (subscription == null)
            {
                throw new Exception("DefaultWebHookSender can not send webhook since could not found web hook subscription by id: " + webHookSubscriptionId);
            }

            var workItemId = await InsertAndGetIdWebHookWorkItemAsync(webhook, subscription);

            var request = CreateWebHookRequestMessage(subscription);

            var webHookBody = await GetWebhookBodyAsync(webhook, subscription);

            var serializedBody = _webHooksConfiguration.JsonSerializerSettings != null
                ? webHookBody.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                : webHookBody.ToJsonString();

            SignWebHookRequest(request, serializedBody, subscription.Secret);

            AddAdditionalHeaders(request, subscription);

            bool isSucceed;
            //TODO:Use client factory
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                await StoreResponseOnWebHookWorkItemAsync(workItemId, response);
                isSucceed = response.IsSuccessStatusCode;
            }

            return isSucceed;
        }

        public bool TrySendWebHook(Guid webHookId, Guid webHookSubscriptionId)
        {
            if (webHookId == default)
            {
                throw new ArgumentNullException(nameof(webHookId));
            }

            if (webHookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(webHookSubscriptionId));
            }

            var webhook = _webhookStoreManager.Get(webHookId);
            if (webhook == null)
            {
                throw new Exception("DefaultWebHookSender can not send webhook since could not found webhook by id: " + webHookId);
            }

            var subscription = _webHookSubscriptionManager.Get(webHookSubscriptionId);
            if (subscription == null)
            {
                throw new Exception("DefaultWebHookSender can not send webhook since could not found web hook subscription by id: " + webHookSubscriptionId);
            }

            var workItemId = InsertAndGetIdWebHookWorkItem(webhook, subscription);

            var request = CreateWebHookRequestMessage(subscription);

            var webHookBody = GetWebhookBody(webhook, subscription);

            var serializedBody = _webHooksConfiguration.JsonSerializerSettings != null
                ? webHookBody.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                : webHookBody.ToJsonString();

            SignWebHookRequest(request, serializedBody, subscription.Secret);

            AddAdditionalHeaders(request, subscription);

            bool isSucceed;
            //TODO:Use client factory
            using (var client = new HttpClient())
            {
                var response = AsyncHelper.RunSync(() => client.SendAsync(request));
                StoreResponseOnWebHookWorkItem(workItemId, response);
                isSucceed = response.IsSuccessStatusCode;
            }

            return isSucceed;
        }

        [UnitOfWork]
        protected virtual async Task<Guid> InsertAndGetIdWebHookWorkItemAsync(WebHookInfo webhook, WebHookSubscription subscription)
        {
            if (!subscription.IsSubscribed(webhook.WebHookDefinition))
            {
                throw new ApplicationException($"Subscription does not contain webhook subscription for {webhook.WebHookDefinition}");
            }

            var workItem = new WebHookWorkItem()
            {
                WebHookId = webhook.Id,
                WebHookSubscriptionId = subscription.Id
            };

            await WebHookWorkItemStore.InsertAsync(workItem);
            await CurrentUnitOfWork.SaveChangesAsync();

            return workItem.Id;
        }

        [UnitOfWork]
        protected virtual Guid InsertAndGetIdWebHookWorkItem(WebHookInfo webhook, WebHookSubscription subscription)
        {
            if (!subscription.IsSubscribed(webhook.WebHookDefinition))
            {
                throw new ApplicationException($"Subscription does not contain webhook subscription for {webhook.WebHookDefinition}");
            }

            var workItem = new WebHookWorkItem()
            {
                WebHookId = webhook.Id,
                WebHookSubscriptionId = subscription.Id
            };

            WebHookWorkItemStore.Insert(workItem);
            CurrentUnitOfWork.SaveChanges();

            return workItem.Id;
        }

        [UnitOfWork]
        protected virtual async Task StoreResponseOnWebHookWorkItemAsync(Guid webHookWorkItemId, HttpResponseMessage responseMessage)
        {
            var webHookWorkItem = await WebHookWorkItemStore.GetAsync(webHookWorkItemId);

            webHookWorkItem.Transmitted = responseMessage.IsSuccessStatusCode;
            webHookWorkItem.ResponseStatusCode = responseMessage.StatusCode;
            webHookWorkItem.ResponseContent = await responseMessage.Content.ReadAsStringAsync();

            await WebHookWorkItemStore.UpdateAsync(webHookWorkItem);
        }

        [UnitOfWork]
        protected virtual void StoreResponseOnWebHookWorkItem(Guid webHookWorkItemId, HttpResponseMessage responseMessage)
        {
            var webHookWorkItem = WebHookWorkItemStore.Get(webHookWorkItemId);

            webHookWorkItem.Transmitted = responseMessage.IsSuccessStatusCode;
            webHookWorkItem.ResponseStatusCode = responseMessage.StatusCode;
            webHookWorkItem.ResponseContent = AsyncHelper.RunSync(() => responseMessage.Content.ReadAsStringAsync());

            WebHookWorkItemStore.Update(webHookWorkItem);
        }

        /// <summary>
        /// You can override this to change request message
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        protected virtual HttpRequestMessage CreateWebHookRequestMessage(WebHookSubscription subscription)
        {
            return new HttpRequestMessage(HttpMethod.Post, subscription.WebHookUri);
        }

        protected virtual async Task<WebhookBody> GetWebhookBodyAsync(WebHookInfo webhook, WebHookSubscription subscription)
        {
            return new WebhookBody
            {
                Event = webhook.WebHookDefinition,
                Data = webhook.Data,
                Attempt = await WebHookWorkItemStore.GetRepetitionCountAsync(webhook.Id, subscription.Id) + 1
            };
        }

        protected virtual WebhookBody GetWebhookBody(WebHookInfo webhook, WebHookSubscription subscription)
        {
            return new WebhookBody
            {
                Event = webhook.WebHookDefinition,
                Data = webhook.Data,
                Attempt = WebHookWorkItemStore.GetRepetitionCount(webhook.Id, subscription.Id) + 1
            };
        }

        protected virtual void SignWebHookRequest(HttpRequestMessage request, string serializedBody, string secret)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(serializedBody))
            {
                throw new ArgumentNullException(nameof(serializedBody));
            }

            var secretBytes = Encoding.UTF8.GetBytes(secret);

            using (var hasher = new HMACSHA256(secretBytes))
            {
                request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");

                var data = Encoding.UTF8.GetBytes(serializedBody);
                var sha256 = hasher.ComputeHash(data);

                var headerValue = string.Format(CultureInfo.InvariantCulture, SignatureHeaderValueTemplate, BitConverter.ToString(sha256));

                request.Headers.Add(SignatureHeaderName, headerValue);
            }
        }

        protected virtual void AddAdditionalHeaders(HttpRequestMessage request, WebHookSubscription subscription)
        {
            foreach (var header in subscription.Headers)
            {
                if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }
                if (request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }

                throw new Exception($"Invalid Header.SubscriptionId:{subscription.Id},Header: {header.Key}:{header.Value}");
            }
        }
    }
}
