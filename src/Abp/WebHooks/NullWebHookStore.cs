using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    /// <summary>
    /// Null pattern implementation of <see cref="IWebHookSubscriptionsStore"/>.
    /// It's used if <see cref="IWebHookSubscriptionsStore"/> is not implemented by actual persistent store
    /// </summary>
    public class NullWebHookStore : IWebHookStore
    {
        public static NullWebHookStore Instance { get; } = new NullWebHookStore();


        public Task InsertAsync(WebHookInfo webHookInfo)
        {
            return Task.CompletedTask;
        }

        public void Insert(WebHookInfo webHookInfo)
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
    }
}
