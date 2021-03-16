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

        public Task<bool> HasXConsecutiveFailAsync(int? tenantId, Guid subscriptionId, int searchCount)
        {
            return default;
        }

        public Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(int? tenantId, Guid subscriptionId, int maxResultCount,
            int skipCount)
        {
            return Task.FromResult(new PagedResultDto<WebhookSendAttempt>() as IPagedResult<WebhookSendAttempt>);
        }

        public IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(int? tenantId, Guid subscriptionId, int maxResultCount,
            int skipCount)
        {
            return new PagedResultDto<WebhookSendAttempt>();
        }

        public Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(int? tenantId, Guid webhookEventId)
        {
            return Task.FromResult(new List<WebhookSendAttempt>());
        }

        public List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(int? tenantId, Guid webhookEventId)
        {
            return new List<WebhookSendAttempt>();
        }
    }
}
