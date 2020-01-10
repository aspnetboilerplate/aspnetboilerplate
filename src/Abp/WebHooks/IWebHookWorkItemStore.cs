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

        Task<WebHookWorkItem> GetAsync(int? tenantId, Guid id);

        WebHookWorkItem Get(int? tenantId, Guid id);

        /// <summary>
        /// Returns work item count by given web hook id and subscription id, (How many times publisher tried to send web hook)
        /// </summary>
        Task<int> GetRepetitionCountAsync(int? tenantId, Guid webHookId, Guid webHookSubscriptionId);

        /// <summary>
        /// Returns work item count by given web hook id and subscription id. (How many times publisher tried to send web hook)
        /// </summary>
        int GetRepetitionCount(int? tenantId, Guid webHookId, Guid webHookSubscriptionId);
    }
}
