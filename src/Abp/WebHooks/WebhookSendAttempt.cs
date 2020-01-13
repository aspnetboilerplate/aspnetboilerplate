using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.Webhooks
{
    /// <summary>
    /// Table for store webhook work items. Each item stores web hook send attempt of <see cref="WebhookInfo"/> to subscribed tenants
    /// </summary>
    [Table("AbpWebhookSendAttempts")]
    public class WebhookSendAttempt : Entity<Guid>, IMayHaveTenant, IHasCreationTime, IHasModificationTime
    {
        /// <summary>
        /// <see cref="WebhookInfo"/> foreign id 
        /// </summary>
        public Guid WebhookId { get; set; }

        /// <summary>
        /// <see cref="WebhookSubscription"/> foreign id 
        /// </summary>
        public Guid WebhookSubscriptionId { get; set; }

        /// <summary>
        /// Webhook response content that webhook endpoint send back
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Webhook response status code that webhook endpoint send back
        /// </summary>
        public HttpStatusCode? ResponseStatusCode { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public int? TenantId { get; set; }
    }
}
