using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookSender
    {
        /// <summary>
        /// Tries to send webhook with given transactionId and stores process in <see cref="WebHookWorkItem"/>
        /// </summary>
        /// <returns>If progress done</returns>
        Task<bool> TrySendWebHookAsync(Guid webHookId, Guid webHookSubscriptionId);

        /// <summary>
        /// Tries to send webhook with given transactionId and stores process in <see cref="WebHookWorkItem"/>
        /// </summary>
        /// <returns>If transaction done</returns>
        bool TrySendWebHook(Guid webHookId, Guid webHookSubscriptionId);
    }
}
