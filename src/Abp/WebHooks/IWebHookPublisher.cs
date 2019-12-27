using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookPublisher
    {
        /// <summary>
        /// Sends webhooks to all subscribed endpoints with given data
        /// </summary>
        /// <param name="webHookName">webhook unique name</param>
        /// <param name="data">data to send</param>
        Task PublishAsync(string webHookName, object data);

        /// <summary>
        /// Sends webhooks to all subscribed endpoints with given data
        /// </summary>
        /// <param name="webHookName">webhook unique name</param>
        /// <param name="data">data to send</param>
        void Publish(string webHookName, object data);
    }
}
