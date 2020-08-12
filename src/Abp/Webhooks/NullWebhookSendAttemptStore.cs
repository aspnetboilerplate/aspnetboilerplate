using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

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

        public Task<WebhookSendAttempt> GetAsync(long? tenantId, Guid id)
        {
            return Task.FromResult<WebhookSendAttempt>(default);
        }

        public WebhookSendAttempt Get(long? tenantId, Guid id)
        {
            return default;
        }

        public Task<int> GetSendAttemptCountAsync(long? tenantId, Guid webhookId, Guid webhookSubscriptionId)
        {
            return Task.FromResult(int.MaxValue);
        }

        public int GetSendAttemptCount(long? tenantId, Guid webhookId, Guid webhookSubscriptionId)
        {
            return int.MaxValue;
        }

        public Task<bool> HasXConsecutiveFailAsync(long? tenantId, Guid subscriptionId, int searchCount)
        {
            return default;
        }

        public bool HasXConsecutiveFail(long? tenantId, Guid subscriptionId, int searchCount)
        {
            return default;
        }

        public Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(long? tenantId, Guid subscriptionId, int maxResultCount,
            int skipCount)
        {
            return Task.FromResult(new PagedResultDto<WebhookSendAttempt>() as IPagedResult<WebhookSendAttempt>);
        }

        public IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(long? tenantId, Guid subscriptionId, int maxResultCount,
            int skipCount)
        {
            return new PagedResultDto<WebhookSendAttempt>();
        }

        public Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(long? tenantId, Guid webhookEventId)
        {
            return Task.FromResult(new List<WebhookSendAttempt>());
        }

        public List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(long? tenantId, Guid webhookEventId)
        {
            return new List<WebhookSendAttempt>();
        }
    }
}
