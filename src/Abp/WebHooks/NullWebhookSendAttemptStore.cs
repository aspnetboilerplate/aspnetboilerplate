using System;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    public class NullWebhookSendAttemptStore : IWebhookSendAttemptStore
    {
        public static NullWebhookSendAttemptStore Instance = new NullWebhookSendAttemptStore();

        public Task InsertAsync(WebhookSendAttempt webhookSendAttempt)
        {
            return Task.CompletedTask;
        }

        public void Insert(WebhookSendAttempt webhookSendAttempt)
        {
        }

        public Task UpdateAsync(WebhookSendAttempt webhookSendAttempt)
        {
            return Task.CompletedTask;
        }

        public void Update(WebhookSendAttempt webhookSendAttempt)
        {
        }

        public Task<WebhookSendAttempt> GetAsync(int? tenantId, Guid id)
        {
            return Task.FromResult<WebhookSendAttempt>(default);
        }

        public WebhookSendAttempt Get(int? tenantId, Guid id)
        {
            return default;
        }

        public Task<int> GetSendAttemptCountAsync(int? tenantId, Guid webhookId, Guid webhookSubscriptionId)
        {
            return Task.FromResult(int.MaxValue);
        }

        public int GetSendAttemptCount(int? tenantId, Guid webhookId, Guid webhookSubscriptionId)
        {
            return int.MaxValue;
        }

        public Task<bool> HasAnySuccessfulAttemptInLastXRecordAsync(int? tenantId, Guid subscriptionId, int searchCount)
        {
            return Task.FromResult(true);
        }

        public bool HasAnySuccessfulAttemptInLastXRecord(int? tenantId, Guid subscriptionId, int searchCount)
        {
            return true;
        }
    }
}
