using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
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

        public Task<int> GetRepetitionCountAsync(int? tenantId, Guid webHookId, Guid webHookSubscriptionId)
        {
            return Task.FromResult(int.MaxValue);
        }

        public int GetRepetitionCount(int? tenantId, Guid webHookId, Guid webHookSubscriptionId)
        {
            return int.MaxValue;
        }
    }
}
