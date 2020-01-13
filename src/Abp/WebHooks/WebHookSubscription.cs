using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;

namespace Abp.WebHooks
{
    public class WebHookSubscription : EntityDto<Guid>, IPassivable
    {
        /// <summary>
        /// Tenant id of the subscribed.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Subscription webhook endpoint
        /// </summary>
        public string WebHookUri { get; set; }

        /// <summary>
        /// Webhook secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Is subscription active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Subscribed webhook definitions unique names.
        /// </summary>
        public List<string> WebHookDefinitions { get; set; }

        /// <summary>
        /// Gets a set of additional HTTP headers.That headers will be sent with the webhook.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        public WebHookSubscription()
        {
            IsActive = true;
        }
    }
}
