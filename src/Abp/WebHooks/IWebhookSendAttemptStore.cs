using System;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    public interface IWebhookSendAttemptStore
    {
        Task InsertAsync(WebhookSendAttempt webhookSendAttempt);

        void Insert(WebhookSendAttempt webhookSendAttempt);

        Task UpdateAsync(WebhookSendAttempt webhookSendAttempt);

        void Update(WebhookSendAttempt webhookSendAttempt);

        Task<WebhookSendAttempt> GetAsync(int? tenantId, Guid id);

        WebhookSendAttempt Get(int? tenantId, Guid id);

        /// <summary>
        /// Returns work item count by given web hook id and subscription id, (How many times publisher tried to send web hook)
        /// </summary>
        Task<int> GetSendAttemptCountAsync(int? tenantId, Guid webhookId, Guid webhookSubscriptionId);

        /// <summary>
        /// Returns work item count by given web hook id and subscription id. (How many times publisher tried to send web hook)
        /// </summary>
        int GetSendAttemptCount(int? tenantId, Guid webhookId, Guid webhookSubscriptionId);

        /// <summary>
        /// Checks is there any successful webhook attempt in last <paramref name="searchCount"/> items. Should return true if there are not X number items
        /// </summary>
        Task<bool> HasAnySuccessfulAttemptInLastXRecordAsync(int? tenantId, Guid subscriptionId, int searchCount);

        /// <summary>
        /// Checks is there any successful webhook attempt in last <paramref name="searchCount"/> items. Should return true if there are not X number items
        /// </summary>
        bool HasAnySuccessfulAttemptInLastXRecord(int? tenantId, Guid subscriptionId, int searchCount);
    }
}
