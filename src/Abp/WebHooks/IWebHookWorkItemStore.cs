using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookWorkItemStore
    {
        Task InsertAsync(WebHookWorkItem webHookWorkItem);

        void Insert(WebHookWorkItem webHookWorkItem);

        Task UpdateAsync(WebHookWorkItem webHookWorkItem);

        void Update(WebHookWorkItem webHookWorkItem);

        Task<WebHookWorkItem> GetAsync(Guid id);

        WebHookWorkItem Get(Guid id);

        /// <summary>
        /// Returns work item count by given web hook id and subscription id, (How many times publisher tried to send web hook)
        /// </summary>
        Task<int> GetRepetitionCountAsync(Guid webHookId, Guid webHookSubscriptionId);

        /// <summary>
        /// Returns work item count by given web hook id and subscription id. (How many times publisher tried to send web hook)
        /// </summary>
        int GetRepetitionCount(Guid webHookId, Guid webHookSubscriptionId);
    }
}
