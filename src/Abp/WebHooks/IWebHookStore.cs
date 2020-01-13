using System;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    public interface IWebhookStore
    {
        /// <summary>
        /// Inserts to persistent store
        /// </summary>
        Task<Guid> InsertAndGetIdAsync(WebhookInfo webhookInfo);

        /// <summary>
        /// Inserts to persistent store
        /// </summary>
        Guid InsertAndGetId(WebhookInfo webhookInfo);

        /// <summary>
        /// Gets Webhook info by id
        /// </summary>
        Task<WebhookInfo> GetAsync(int? tenantId, Guid id);

        /// <summary>
        /// Gets Webhook info by id
        /// </summary>
        WebhookInfo Get(int? tenantId, Guid id);
    }
}
