using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Domain.Uow;

namespace Abp.Webhooks
{
    public class DefaultWebhookSender : DomainService, IWebhookSender
    {
        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly IWebhookManager _webhookManager;
        
        private const string FailedRequestDefaultContent = "Webhook Send Request Failed";

        public DefaultWebhookSender(
            IWebhooksConfiguration webhooksConfiguration, 
            IWebhookManager webhookManager)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _webhookManager = webhookManager;
        }

        public async Task<Guid> SendWebhookAsync(WebhookSenderArgs webhookSenderArgs)
        {
            if (webhookSenderArgs.WebhookEventId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookEventId));
            }

            if (webhookSenderArgs.WebhookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(webhookSenderArgs.WebhookSubscriptionId));
            }

            var webhookSendAttemptId = await _webhookManager.InsertAndGetIdWebhookSendAttemptAsync(webhookSenderArgs);

            var request = CreateWebhookRequestMessage(webhookSenderArgs);

            var serializedBody = await _webhookManager.GetSerializedBodyAsync(webhookSenderArgs);

            _webhookManager.SignWebhookRequest(request, serializedBody, webhookSenderArgs.Secret);

            AddAdditionalHeaders(request, webhookSenderArgs);

            var isSucceed = false;
            HttpStatusCode? statusCode = null;
            var content = FailedRequestDefaultContent;

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
                await _webhookManager.StoreResponseOnWebhookSendAttemptAsync(webhookSendAttemptId, webhookSenderArgs.TenantId, statusCode, content);
            }

            if (!isSucceed)
            {
                throw new Exception($"Webhook sending attempt failed. WebhookSendAttempt id: {webhookSendAttemptId}");
            }

            return webhookSendAttemptId;
        }

        /// <summary>
        /// You can override this to change request message
        /// </summary>
        /// <returns></returns>
        protected virtual HttpRequestMessage CreateWebhookRequestMessage(WebhookSenderArgs webhookSenderArgs)
        {
            return new HttpRequestMessage(HttpMethod.Post, webhookSenderArgs.WebhookUri);
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
    }
}
