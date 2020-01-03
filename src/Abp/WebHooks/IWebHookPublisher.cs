using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookPublisher
    {
        /// <summary>
        /// Sends webhooks to all subscribed endpoints with given data
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        Task PublishAsync(string webHookName, object data);

        /// <summary>
        /// Sends webhooks to all subscribed endpoints with given data
        /// </summary>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        void Publish(string webHookName, object data);

        /// <summary>
        /// Sends webhooks to subscriptions of given user with given data if he subscribed to that webhook and has permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        Task PublishAsync(UserIdentifier user, string webHookName, object data);

        /// <summary>
        /// Sends webhooks to subscriptions of given user with given data if he subscribed to that webhook and has permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="webHookName"><see cref="WebHookDefinition.Name"/></param>
        /// <param name="data">data to send</param>
        void Publish(UserIdentifier user, string webHookName, object data);
    }
}
