using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Abp.WebHooks
{
    [Table("AbpWebHookSubscriptions")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class WebHookSubscriptionInfo : CreationAuditedEntity<Guid>, IPassivable
    {
        /// <summary>
        /// Subscribed Tenant's id .
        /// </summary>
        public int? TenantId { get; set; }
        
        /// <summary>
        /// Subscription webhook endpoint
        /// </summary>
        [Required]
        public string WebHookUri { get; set; }

        /// <summary>
        /// Webhook secret
        /// </summary>
        [Required]
        public string Secret { get; set; }

        /// <summary>
        /// Is subscription active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Subscribed webhook definitions unique names.It contains webhook definitions list as json
        /// <para>
        /// Do not change it manually. Use <see cref="WebHookSubscriptionExtensions.SubscribeWebhook"/>,
        /// <see cref="WebHookSubscriptionExtensions.UnsubscribeWebhook"/> and
        /// <see cref="WebHookSubscriptionExtensions.RemoveAllSubscribedWebhooks"/> to change it.
        /// </para> 
        /// </summary>
        public string WebHookDefinitions { get; set; }

        /// <summary>
        /// Gets a set of additional HTTP headers.That headers will be sent with the webhook. It contains webhook header dictionary as json
        /// </summary>
        public string Headers { get; set; }

        public WebHookSubscriptionInfo()
        {
            IsActive = true;
        }
    }
}
