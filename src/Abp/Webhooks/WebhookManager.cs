using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Json;

namespace Abp.Webhooks
{
    public class WebhookManager : DomainService, IWebhookManager
    {
        private const string SignatureHeaderKey = "sha256";
        private const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";
        private const string SignatureHeaderName = "abp-webhook-signature";
        
        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly IWebhookSendAttemptStore _webhookSendAttemptStore;

        public WebhookManager(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookSendAttemptStore webhookSendAttemptStore)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _webhookSendAttemptStore = webhookSendAttemptStore;
        }

        public virtual async Task<WebhookPayload> GetWebhookPayloadAsync(WebhookSenderArgs webhookSenderArgs)
        {
            var data = _webhooksConfiguration.JsonSerializerSettings != null
                ? webhookSenderArgs.Data.FromJsonString<dynamic>(_webhooksConfiguration.JsonSerializerSettings)
                : webhookSenderArgs.Data.FromJsonString<dynamic>();

            var attemptNumber = await _webhookSendAttemptStore.GetSendAttemptCountAsync(
                webhookSenderArgs.TenantId,
                webhookSenderArgs.WebhookEventId,
                webhookSenderArgs.WebhookSubscriptionId);

            return new WebhookPayload(
                webhookSenderArgs.WebhookEventId.ToString(),
                webhookSenderArgs.WebhookName,
                attemptNumber)
            {
                Data = data
            };
        }

        public virtual WebhookPayload GetWebhookPayload(WebhookSenderArgs webhookSenderArgs)
        {
            var data = _webhooksConfiguration.JsonSerializerSettings != null
                ? webhookSenderArgs.Data.FromJsonString<dynamic>(_webhooksConfiguration.JsonSerializerSettings)
                : webhookSenderArgs.Data.FromJsonString<dynamic>();

            var attemptNumber = _webhookSendAttemptStore.GetSendAttemptCount(
                webhookSenderArgs.TenantId,
                webhookSenderArgs.WebhookEventId,
                webhookSenderArgs.WebhookSubscriptionId) + 1;

            return new WebhookPayload(
                webhookSenderArgs.WebhookEventId.ToString(),
                webhookSenderArgs.WebhookName,
                attemptNumber)
            {
                Data = data
            };
        }

        public virtual void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret)
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

        public virtual string GetSerializedBody(WebhookSenderArgs webhookSenderArgs)
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

        public virtual async Task<string> GetSerializedBodyAsync(WebhookSenderArgs webhookSenderArgs)
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
        
        public virtual async Task<Guid> InsertAndGetIdWebhookSendAttemptAsync(WebhookSenderArgs webhookSenderArgs)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var workItem = new WebhookSendAttempt
                {
                    WebhookEventId = webhookSenderArgs.WebhookEventId,
                    WebhookSubscriptionId = webhookSenderArgs.WebhookSubscriptionId,
                    TenantId = webhookSenderArgs.TenantId
                };

                await _webhookSendAttemptStore.InsertAsync(workItem);
                await CurrentUnitOfWork.SaveChangesAsync();

                await uow.CompleteAsync();
                
                return workItem.Id;
            }
        }

        public virtual async Task StoreResponseOnWebhookSendAttemptAsync(Guid webhookSendAttemptId, int? tenantId, HttpStatusCode? statusCode, string content)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var webhookSendAttempt = await _webhookSendAttemptStore.GetAsync(tenantId, webhookSendAttemptId);

                webhookSendAttempt.ResponseStatusCode = statusCode;
                webhookSendAttempt.Response = content;

                await _webhookSendAttemptStore.UpdateAsync(webhookSendAttempt);

                await uow.CompleteAsync();
            }
        }
    }
}
