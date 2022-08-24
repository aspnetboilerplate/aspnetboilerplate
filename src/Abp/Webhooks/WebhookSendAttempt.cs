using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Abp.Webhooks
{
    /// <summary>
    /// Table for store webhook work items. Each item stores web hook send attempt of <see cref="WebhookEvent"/> to subscribed tenants
    /// </summary>
    [Table("AbpWebhookSendAttempts")]
    public class WebhookSendAttempt : Entity<Guid>, IMayHaveTenant, IHasCreationTime, IHasModificationTime
    {
        /// <summary>
        /// <see cref="WebhookEvent"/> foreign id 
        /// </summary>
        [Required]
        public virtual Guid WebhookEventId { get; set; }

        /// <summary>
        /// <see cref="WebhookSubscription"/> foreign id 
        /// </summary>
        [Required]
        public virtual Guid WebhookSubscriptionId { get; set; }

        /// <summary>
        /// Webhook response content that webhook endpoint send back
        /// </summary>
        public virtual string Response { get; set; }

        /// <summary>
        /// Webhook response status code that webhook endpoint send back
        /// </summary>
        public virtual HttpStatusCode? ResponseStatusCode { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual DateTime? LastModificationTime { get; set; }

        public virtual int? TenantId { get; set; }

        /// <summary>
        /// WebhookEvent of this send attempt.
        /// </summary>
        [ForeignKey("WebhookEventId")]
        public virtual WebhookEvent WebhookEvent { get; set; }
    }
}
