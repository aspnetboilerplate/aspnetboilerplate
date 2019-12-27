using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Abp.WebHooks
{
    public class WebHookSubscription : EntityDto<Guid>
    {
        /// <summary>
        /// Tenant id of the subscribed user.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Subscription webhook endpoint
        /// </summary>
        public Uri WebHookUri { get; set; }

        /// <summary>
        /// User webhook secret
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

        public WebHookSubscription()
        {
            IsActive = true;
        }
    }
}
