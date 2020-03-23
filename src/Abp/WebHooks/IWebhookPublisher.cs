using System.Threading.Tasks;
using Abp.Runtime.Session;

namespace Abp.Webhooks
{
    public interface IWebhookPublisher
    {
        /// <summary>
        /// Sends webhooks to current tenant subscriptions (<see cref="IAbpSession.TenantId"/>). with given data, (Checks permissions)
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        Task PublishAsync(string webhookName, object data);
        
        /// <summary>
        /// Sends webhooks to current tenant subscriptions (<see cref="IAbpSession.TenantId"/>). with given data, (Checks permissions)
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        void Publish(string webhookName, object data);

        /// <summary>
        /// Sends webhooks to given tenant's subscriptions
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        /// <param name="tenantId">
        /// Target tenant id
        /// </param>
        Task PublishAsync(string webhookName, object data, int? tenantId);
        
        /// <summary>
        /// Sends webhooks to given tenant's subscriptions
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        /// <param name="tenantId">
        /// Target tenant id
        /// </param>
        void Publish(string webhookName, object data, int? tenantId);
    }
}
