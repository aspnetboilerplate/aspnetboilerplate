using System.Threading.Tasks;
using Abp.Runtime.Session;

namespace Abp.WebHooks
{
    public interface IWebHookPublisher
    {
        /// <summary>
        /// Sends webhooks to current tenant subscriptions(<see cref="IAbpSession.TenantId"/>). with given data, (Checks permissions)
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        Task PublishAsync(string webHookName, object data);
        
        /// <summary>
        /// Sends webhooks to current tenant subscriptions(<see cref="IAbpSession.TenantId"/>). with given data, (Checks permissions)
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        void Publish(string webHookName, object data);

        /// <summary>
        /// Sends webhooks to given tenant's subscriptions
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        Task PublishAsync(string webHookName, object data, int? tenantId);
        
        /// <summary>
        /// Sends webhooks to given tenant's subscriptions
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        /// <param name="tenantId">
        /// Target tenant id(s).
        /// </param>
        void Publish(string webHookName, object data, int? tenantId);
    }
}
