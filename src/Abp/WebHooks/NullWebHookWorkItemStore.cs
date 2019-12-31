using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public class NullWebHookWorkItemStore : IWebHookWorkItemStore
    {
        public static NullWebHookWorkItemStore Instance = new NullWebHookWorkItemStore();

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

        public Task<WebHookWorkItem> GetAsync(Guid id)
        {
            return Task.FromResult<WebHookWorkItem>(default);
        }

        public WebHookWorkItem Get(Guid id)
        {
            return default;
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
