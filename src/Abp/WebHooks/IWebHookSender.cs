using System;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    public interface IWebhookSender
    {
        /// <summary>
        /// Tries to send webhook with given transactionId and stores process in <see cref="WebhookSendAttempt"/>
        /// </summary>
        /// <returns>If progress done</returns>
        Task<bool> TrySendWebhookAsync(WebhookSenderInput webhookSenderArgs);

        /// <summary>
        /// Tries to send webhook with given transactionId and stores process in <see cref="WebhookSendAttempt"/>
        /// </summary>
        /// <returns>If transaction done</returns>
        bool TrySendWebhook(WebhookSenderInput webhookSenderArgs);
    }
}
