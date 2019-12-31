using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    /// <summary>
    /// Uses <see cref="IWebHookStore"/>, works with cache
    /// </summary>
    public interface IWebhookStoreManager
    {
        /// <summary>
        /// Inserts to persistent store
        /// </summary>
        Task<Guid> InsertAndGetIdAsync(WebHookInfo webHookInfo);

        /// <summary>
        /// Inserts to persistent store
        /// </summary>
        Guid InsertAndGetId(WebHookInfo webHookInfo);

        /// <summary>
        /// Gets Webhook info by id
        /// </summary>
        Task<WebHookInfo> GetAsync(Guid id);

        /// <summary>
        /// Gets Webhook info by id
        /// </summary>
        WebHookInfo Get(Guid id);
    }
}
