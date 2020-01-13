using System;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookSender
    {
        /// <summary>
        /// Tries to send webhook with given transactionId and stores process in <see cref="WebhookSendAttempt"/>
        /// </summary>
        /// <returns>If progress done</returns>
        Task<bool> TrySendWebHookAsync(WebHookSenderInput webHookSenderArgs);

        /// <summary>
        /// Tries to send webhook with given transactionId and stores process in <see cref="WebhookSendAttempt"/>
        /// </summary>
        /// <returns>If transaction done</returns>
        bool TrySendWebHook(WebHookSenderInput webHookSenderArgs);
    }
}
