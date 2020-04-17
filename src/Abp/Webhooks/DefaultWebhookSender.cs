using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.Threading;

namespace Abp.Webhooks
{
    public class DefaultWebhookSender : DomainService, IWebhookSender
    {
        public IWebhookSendAttemptStore WebhookSendAttemptStore { get; set; }

        protected const string SignatureHeaderKey = "sha256";
        protected const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";
        protected const string SignatureHeaderName = "abp-webhook-signature";

        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private const string FailedRequestDefaultContent = "Webhook Send Request Failed";

        public DefaultWebhookSender(IWebhooksConfiguration webhooksConfiguration)
        {
            _webhooksConfiguration = webhooksConfiguration;

            WebhookSendAttemptStore = NullWebhookSendAttemptStore.Instance;
        }

        public async Task SendWebhookAsync(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.WebhookEventId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookEventId));
            }

            if (webhookSenderArgs.WebhookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookSubscriptionId));
            }

            var webhookSendAttemptId = await InsertAndGetIdWebhookSendAttemptAsync(webhookSenderArgs);

            var request = CreateWebhookRequestMessage(webhookSenderArgs);

            var serializedBody = await GetSerializedBodyAsync(webhookSenderArgs);

            SignWebhookRequest(request, serializedBody, webhookSenderArgs.Secret);

            AddAdditionalHeaders(request, webhookSenderArgs);

            bool isSucceed = false;
            HttpStatusCode? statusCode = null;
            string content = FailedRequestDefaultContent;

            try
            {
                var response = await SendHttpRequest(request);
                isSucceed = response.isSucceed;
                statusCode = response.statusCode;
                content = response.content;
            }
            catch (TaskCanceledException)
            {
                statusCode = HttpStatusCode.RequestTimeout;
                content = "Request Timeout";
            }
            catch (HttpRequestException e)
            {
                content = e.Message;
            }
            catch (Exception e)
            {
                Logger.Error("An error occured while sending a webhook request", e);
            }
            finally
            {
                await StoreResponseOnWebhookSendAttemptAsync(webhookSendAttemptId, webhookSenderArgs.TenantId, statusCode, content);
            }

            if (!isSucceed)
            {
                throw new Exception($"Webhook sending attempt failed. WebhookSendAttempt id: {webhookSendAttemptId}");
            }
        }

        public void SendWebhook(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.WebhookEventId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookEventId));
            }

            if (webhookSenderArgs.WebhookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookSubscriptionId));
            }

            var webhookSendAttemptId = InsertAndGetIdWebhookSendAttempt(webhookSenderArgs);

            var request = CreateWebhookRequestMessage(webhookSenderArgs);

            var serializedBody = GetSerializedBody(webhookSenderArgs);

            SignWebhookRequest(request, serializedBody, webhookSenderArgs.Secret);

            AddAdditionalHeaders(request, webhookSenderArgs);

            bool isSucceed = false;
            HttpStatusCode? statusCode = null;
            string content = FailedRequestDefaultContent;

            try
            {
                var response = AsyncHelper.RunSync(() => SendHttpRequest(request));
                isSucceed = response.isSucceed;
                statusCode = response.statusCode;
                content = response.content;
            }
            catch (TaskCanceledException)
            {
                statusCode = HttpStatusCode.RequestTimeout;
                content = "Request Timeout";
            }
            catch (HttpRequestException e)
            {
                content = e.Message;
            }
            catch (Exception e)
            {
                Logger.Error("An error occured while sending a webhook request", e);
            }
            finally
            {
                StoreResponseOnWebhookSendAttempt(webhookSendAttemptId, webhookSenderArgs.TenantId, statusCode, content);
            }

            if (!isSucceed)
            {
                throw new Exception($"Webhook send attempt failed. WebhookSendAttempt id: {webhookSendAttemptId}");
            }
        }

        [UnitOfWork]
        protected virtual async Task<Guid> InsertAndGetIdWebhookSendAttemptAsync(WebhookSenderArgs webhookSenderArgs)
        {
            var workItem = new WebhookSendAttempt
            {
                WebhookEventId = webhookSenderArgs.WebhookEventId,
                WebhookSubscriptionId = webhookSenderArgs.WebhookSubscriptionId,
                TenantId = webhookSenderArgs.TenantId
            };

            await WebhookSendAttemptStore.InsertAsync(workItem);
            await CurrentUnitOfWork.SaveChangesAsync();

            return workItem.Id;
        }

        [UnitOfWork]
        protected virtual Guid InsertAndGetIdWebhookSendAttempt(WebhookSenderArgs webhookSenderArgs)
        {
            var workItem = new WebhookSendAttempt
            {
                WebhookEventId = webhookSenderArgs.WebhookEventId,
                WebhookSubscriptionId = webhookSenderArgs.WebhookSubscriptionId,
                TenantId = webhookSenderArgs.TenantId
            };

            WebhookSendAttemptStore.Insert(workItem);
            CurrentUnitOfWork.SaveChanges();

            return workItem.Id;
        }

        [UnitOfWork]
        protected virtual async Task StoreResponseOnWebhookSendAttemptAsync(Guid webhookSendAttemptId, int? tenantId, HttpStatusCode? statusCode, string content)
        {
            var webhookSendAttempt = await WebhookSendAttemptStore.GetAsync(tenantId, webhookSendAttemptId);

            webhookSendAttempt.ResponseStatusCode = statusCode;
            webhookSendAttempt.Response = content;

            await WebhookSendAttemptStore.UpdateAsync(webhookSendAttempt);
        }

        [UnitOfWork]
        protected virtual void StoreResponseOnWebhookSendAttempt(Guid webhookSendAttemptId, int? tenantId, HttpStatusCode? statusCode, string content)
        {
            var webhookSendAttempt = WebhookSendAttemptStore.Get(tenantId, webhookSendAttemptId);

            webhookSendAttempt.ResponseStatusCode = statusCode;
            webhookSendAttempt.Response = content;

            WebhookSendAttemptStore.Update(webhookSendAttempt);
        }

        /// <summary>
        /// You can override this to change request message
        /// </summary>
        /// <returns></returns>
        protected virtual HttpRequestMessage CreateWebhookRequestMessage(WebhookSenderArgs webhookSenderArgs)
        {
            return new HttpRequestMessage(HttpMethod.Post, webhookSenderArgs.WebhookUri);
        }

        protected virtual async Task<WebhookPayload> GetWebhookPayloadAsync(WebhookSenderArgs webhookSenderArgs)
        {
            dynamic data = _webhooksConfiguration.JsonSerializerSettings != null
                ? webhookSenderArgs.Data.FromJsonString<dynamic>(_webhooksConfiguration.JsonSerializerSettings)
                : webhookSenderArgs.Data.FromJsonString<dynamic>();

            var attemptNumber = await WebhookSendAttemptStore.GetSendAttemptCountAsync(webhookSenderArgs.TenantId,
                                    webhookSenderArgs.WebhookEventId, webhookSenderArgs.WebhookSubscriptionId) + 1;

            return new WebhookPayload(webhookSenderArgs.WebhookEventId.ToString(), webhookSenderArgs.WebhookName, attemptNumber)
            {
                Data = data
            };
        }

        protected virtual WebhookPayload GetWebhookPayload(WebhookSenderArgs webhookSenderArgs)
        {
            dynamic data = _webhooksConfiguration.JsonSerializerSettings != null
                ? webhookSenderArgs.Data.FromJsonString<dynamic>(_webhooksConfiguration.JsonSerializerSettings)
                : webhookSenderArgs.Data.FromJsonString<dynamic>();

            var attemptNumber = WebhookSendAttemptStore.GetSendAttemptCount(webhookSenderArgs.TenantId,
                                    webhookSenderArgs.WebhookEventId, webhookSenderArgs.WebhookSubscriptionId) + 1;

            return new WebhookPayload(webhookSenderArgs.WebhookEventId.ToString(), webhookSenderArgs.WebhookName, attemptNumber)
            {
                Data = data
            };
        }

        protected virtual void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret)
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

        protected virtual void AddAdditionalHeaders(HttpRequestMessage request, WebhookSenderArgs webhookSenderArgs)
        {
            foreach (var header in webhookSenderArgs.Headers)
            {
                if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }

                if (request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    continue;
                }

                throw new Exception($"Invalid Header. SubscriptionId:{webhookSenderArgs.WebhookSubscriptionId},Header: {header.Key}:{header.Value}");
            }
        }

        protected virtual async Task<(bool isSucceed, HttpStatusCode statusCode, string content)> SendHttpRequest(HttpRequestMessage request)
        {
            using (var client = new HttpClient
            {
                Timeout = _webhooksConfiguration.TimeoutDuration
            })
            {
                var response = await client.SendAsync(request);

                var isSucceed = response.IsSuccessStatusCode;
                var statusCode = response.StatusCode;
                var content = await response.Content.ReadAsStringAsync();

                return (isSucceed, statusCode, content);
            }
        }

        protected virtual string GetSerializedBody(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.SendExactSameData)
            {
                return webhookSenderArgs.Data;
            }

            var payload = GetWebhookPayload(webhookSenderArgs);

            var serializedBody = _webhooksConfiguration.JsonSerializerSettings != null
                ? payload.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                : payload.ToJsonString();

            return serializedBody;
        }

        protected virtual async Task<string> GetSerializedBodyAsync(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.SendExactSameData)
            {
                return webhookSenderArgs.Data;
            }

            var payload = await GetWebhookPayloadAsync(webhookSenderArgs);

            var serializedBody = _webhooksConfiguration.JsonSerializerSettings != null
                ? payload.ToJsonString(_webhooksConfiguration.JsonSerializerSettings)
                : payload.ToJsonString();

            return serializedBody;
        }
    }
}
