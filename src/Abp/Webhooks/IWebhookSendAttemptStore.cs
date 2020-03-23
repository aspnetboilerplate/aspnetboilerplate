using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

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
        Task<bool> HasXConsecutiveFailAsync(int? tenantId, Guid subscriptionId, int searchCount);

        /// <summary>
        /// Checks is there any successful webhook attempt in last <paramref name="searchCount"/> items. Should return true if there are not X number items
        /// </summary>
        bool HasXConsecutiveFail(int? tenantId, Guid subscriptionId, int searchCount);

        Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(int? tenantId, Guid subscriptionId, int maxResultCount, int skipCount);

        IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(int? tenantId, Guid subscriptionId, int maxResultCount, int skipCount);

        Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(int? tenantId, Guid webhookEventId);

        List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(int? tenantId, Guid webhookEventId);

    }
}
