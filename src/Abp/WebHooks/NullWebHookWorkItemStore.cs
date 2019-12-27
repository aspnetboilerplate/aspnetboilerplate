using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public class NullWebHookWorkItemStore : IWebHookWorkItemStore
    {
        public Task InsertAsync(WebHookWorkItem webHookWorkItem)
        {
            return Task.CompletedTask;
        }

        public void Insert(WebHookWorkItem webHookWorkItem)
        {
        }

        public Task UpdateAsync(WebHookWorkItem webHookWorkItem)
        {
            return Task.CompletedTask;
        }

        public void Update(WebHookWorkItem webHookWorkItem)
        {
        }

        public Task<WebHookInfo> GetAsync(Guid id)
        {
            return Task.FromResult<WebHookInfo>(null);
        }

        public WebHookInfo Get(Guid id)
        {
            return null;
        }

        public Task<int> GetRepetitionCountAsync(Guid webHookId, Guid webHookSubscriptionId)
        {
            return Task.FromResult(int.MaxValue);
        }

        public int GetRepetitionCount(Guid webHookId, Guid webHookSubscriptionId)
        {
            return int.MaxValue;
        }
    }
}
