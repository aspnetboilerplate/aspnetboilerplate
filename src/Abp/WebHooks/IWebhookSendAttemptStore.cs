using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
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
        Task<int> GetSendAttemptCountAsync(int? tenantId, Guid webHookId, Guid webHookSubscriptionId);

        /// <summary>
        /// Returns work item count by given web hook id and subscription id. (How many times publisher tried to send web hook)
        /// </summary>
        int GetSendAttemptCount(int? tenantId, Guid webHookId, Guid webHookSubscriptionId);
    }
}
