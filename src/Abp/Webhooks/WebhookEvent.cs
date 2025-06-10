using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.Webhooks
{
    /// <summary>
    /// Store created web hooks. To see who get that webhook check with <see cref="WebhookSendAttempt.WebhookEventId"/> and you can get <see cref="WebhookSendAttempt.WebhookSubscriptionId"/>
    /// </summary>
    [Table("AbpWebhookEvents")]
    public class WebhookEvent : Entity<Guid>, IMayHaveTenant, IHasCreationTime, IHasDeletionTime
    {
        /// <summary>
        /// Webhook unique name <see cref="WebhookDefinition.Name"/>
        /// </summary>
        [Required]
        public virtual string WebhookName { get; set; }

        /// <summary>
        /// Webhook data as JSON string.
        /// </summary>
        public virtual string Data { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual int? TenantId { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual DateTime? DeletionTime { get; set; }
    }
}
