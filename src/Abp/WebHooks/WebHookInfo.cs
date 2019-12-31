using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Abp.WebHooks
{
    /// <summary>
    /// Store created web hooks. To see who get that webhook check with <see cref="WebHookWorkItem.WebHookId"/> and you can get <see cref="WebHookWorkItem.WebHookSubscriptionId"/>
    /// </summary>
    [Table("AbpWebHooks")]
    public class WebHookInfo : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// Webhook unique name id
        /// </summary>
        public string WebHookDefinition { get; set; }

        /// <summary>
        /// WebHook data as JSON string.
        /// </summary>
        public string Data { get; set; }
    }
}
