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

        private readonly IWebHooksConfiguration _webHooksConfiguration;
        public IWebHookWorkItemStore WebHookWorkItemStore { get; set; }

        public DefaultWebHookSender(IWebHooksConfiguration webHooksConfiguration)
        {
            _webHooksConfiguration = webHooksConfiguration;

            WebHookWorkItemStore = NullWebHookWorkItemStore.Instance;
        }

        public async Task<bool> TrySendWebHookAsync(WebHookSenderInput webHookSenderArgs)
        {
            try
            {
                if (webHookSenderArgs.WebHookId == default)
                {
                    throw new ArgumentNullException(nameof(webHookSenderArgs.WebHookId));
                }

                if (webHookSenderArgs.WebHookSubscriptionId == default)
                {
                    throw new ArgumentNullException(nameof(webHookSenderArgs.WebHookSubscriptionId));
                }

                var workItemId = await InsertAndGetIdWebHookWorkItemAsync(webHookSenderArgs);

                var request = CreateWebHookRequestMessage(webHookSenderArgs);

                var webHookBody = await GetWebhookBodyAsync(webHookSenderArgs);

                var serializedBody = _webHooksConfiguration.JsonSerializerSettings != null
                    ? webHookBody.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                    : webHookBody.ToJsonString();

                SignWebHookRequest(request, serializedBody, webHookSenderArgs.Secret);

                AddAdditionalHeaders(request, webHookSenderArgs);

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
            catch (Exception e)
            {
                Logger.Error("Error while sending web hook", e);
                return false;
            }
        }

        public bool TrySendWebHook(WebHookSenderInput webHookSenderArgs)
        {
            try
            {
                if (webHookSenderArgs.WebHookId == default)
                {
                    throw new ArgumentNullException(nameof(webHookSenderArgs.WebHookId));
                }

                if (webHookSenderArgs.WebHookSubscriptionId == default)
                {
                    throw new ArgumentNullException(nameof(webHookSenderArgs.WebHookSubscriptionId));
                }

                var workItemId = InsertAndGetIdWebHookWorkItem(webHookSenderArgs);

                var request = CreateWebHookRequestMessage(webHookSenderArgs);

                var webHookBody = GetWebhookBody(webHookSenderArgs);

                var serializedBody = _webHooksConfiguration.JsonSerializerSettings != null
                    ? webHookBody.ToJsonString(_webHooksConfiguration.JsonSerializerSettings)
                    : webHookBody.ToJsonString();

                SignWebHookRequest(request, serializedBody, webHookSenderArgs.Secret);

                AddAdditionalHeaders(request, webHookSenderArgs);

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
            catch (Exception e)
            {
                Logger.Error("Error while sending web hook", e);
                return false;
            }
        }

        [UnitOfWork]
        protected virtual async Task<Guid> InsertAndGetIdWebHookWorkItemAsync(WebHookSenderInput webHookSenderArgs)
        {
            var workItem = new WebHookWorkItem()
            {
                WebHookId = webHookSenderArgs.WebHookId,
                WebHookSubscriptionId = webHookSenderArgs.WebHookSubscriptionId
            };

            await WebHookWorkItemStore.InsertAsync(workItem);
            await CurrentUnitOfWork.SaveChangesAsync();

            return workItem.Id;
        }

        [UnitOfWork]
        protected virtual Guid InsertAndGetIdWebHookWorkItem(WebHookSenderInput webHookSenderArgs)
        {
            var workItem = new WebHookWorkItem()
            {
                WebHookId = webHookSenderArgs.WebHookId,
                WebHookSubscriptionId = webHookSenderArgs.WebHookSubscriptionId
            };

            WebHookWorkItemStore.Insert(workItem);
            CurrentUnitOfWork.SaveChanges();

            return workItem.Id;
        }

        [UnitOfWork]
        protected virtual async Task StoreResponseOnWebHookWorkItemAsync(Guid webHookWorkItemId, HttpResponseMessage responseMessage)
        {
            var webHookWorkItem = await WebHookWorkItemStore.GetAsync(webHookWorkItemId);

            webHookWorkItem.ResponseStatusCode = responseMessage.StatusCode;
            webHookWorkItem.ResponseContent = await responseMessage.Content.ReadAsStringAsync();

            await WebHookWorkItemStore.UpdateAsync(webHookWorkItem);
        }

        [UnitOfWork]
        protected virtual void StoreResponseOnWebHookWorkItem(Guid webHookWorkItemId, HttpResponseMessage responseMessage)
        {
            var webHookWorkItem = WebHookWorkItemStore.Get(webHookWorkItemId);

            webHookWorkItem.ResponseStatusCode = responseMessage.StatusCode;
            webHookWorkItem.ResponseContent = AsyncHelper.RunSync(() => responseMessage.Content.ReadAsStringAsync());

            WebHookWorkItemStore.Update(webHookWorkItem);
        }

        /// <summary>
        /// You can override this to change request message
        /// </summary>
        /// <returns></returns>
        protected virtual HttpRequestMessage CreateWebHookRequestMessage(WebHookSenderInput webHookSenderArgs)
        {
            return new HttpRequestMessage(HttpMethod.Post, webHookSenderArgs.WebHookUri);
        }

        protected virtual async Task<WebhookBody> GetWebhookBodyAsync(WebHookSenderInput webHookSenderArgs)
        {
            return new WebhookBody
            {
                Event = webHookSenderArgs.WebHookDefinition,
                Data = webHookSenderArgs.Data,
                Attempt = await WebHookWorkItemStore.GetRepetitionCountAsync(webHookSenderArgs.WebHookId, webHookSenderArgs.WebHookSubscriptionId) + 1
            };
        }

        protected virtual WebhookBody GetWebhookBody(WebHookSenderInput webHookSenderArgs)
        {
            return new WebhookBody
            {
                Event = webHookSenderArgs.WebHookDefinition,
                Data = webHookSenderArgs.Data,
                Attempt = WebHookWorkItemStore.GetRepetitionCount(webHookSenderArgs.WebHookId, webHookSenderArgs.WebHookSubscriptionId) + 1
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

        protected virtual void AddAdditionalHeaders(HttpRequestMessage request, WebHookSenderInput webHookSenderArgs)
        {
            foreach (var header in webHookSenderArgs.Headers)
            {
                if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }
                if (request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }

                throw new Exception($"Invalid Header. SubscriptionId:{webHookSenderArgs.WebHookSubscriptionId},Header: {header.Key}:{header.Value}");
            }
        }
    }
}
