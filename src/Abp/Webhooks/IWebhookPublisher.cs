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
        /// <param name="sendExactSameData">
        /// True: It sends the exact same data as the parameter to clients.
        /// <para>
        /// False: It sends data in <see cref="WebhookPayload"/>. It is recommended way.
        /// </para>
        /// </param>
        Task PublishAsync(string webhookName, object data, bool sendExactSameData = false);

        /// <summary>
        /// Sends webhooks to current tenant subscriptions (<see cref="IAbpSession.TenantId"/>). with given data, (Checks permissions)
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        /// <param name="sendExactSameData">
        /// True: It sends the exact same data as the parameter to clients.
        /// <para>
        /// False: It sends data in <see cref="WebhookPayload"/>. It is recommended way.
        /// </para>
        /// </param>
        void Publish(string webhookName, object data, bool sendExactSameData = false);

        /// <summary>
        /// Sends webhooks to given tenant's subscriptions
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        /// <param name="tenantId">
        /// Target tenant id
        /// </param>
        /// <param name="sendExactSameData">
        /// True: It sends the exact same data as the parameter to clients.
        /// <para>
        /// False: It sends data in <see cref="WebhookPayload"/>. It is recommended way.
        /// </para>
        /// </param>
        Task PublishAsync(string webhookName, object data, int? tenantId, bool sendExactSameData = false);

        /// <summary>
        /// Sends webhooks to given tenant's subscriptions
        /// </summary>
        /// <param name="webhookName"><see cref="WebhookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        /// <param name="tenantId">
        /// Target tenant id
        /// </param>
        /// <param name="sendExactSameData">
        /// True: It sends the exact same data as the parameter to clients.
        /// <para>
        /// False: It sends data in <see cref="WebhookPayload"/>. It is recommended way.
        /// </para>
        /// </param>
        void Publish(string webhookName, object data, int? tenantId, bool sendExactSameData = false);
    }
}
