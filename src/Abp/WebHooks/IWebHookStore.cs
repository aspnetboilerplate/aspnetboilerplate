using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookStore
    {
        Task InsertAsync(WebHookInfo webHookInfo);

        void Insert(WebHookInfo webHookInfo);

        Task<WebHookInfo> GetAsync(Guid id);

        WebHookInfo Get(Guid id);
    }
}
