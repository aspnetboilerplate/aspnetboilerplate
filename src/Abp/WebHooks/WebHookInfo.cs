using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.WebHooks
{
    [Table("AbpWebHooks")]
    public class WebHookInfo : CreationAuditedEntity<Guid>, ISoftDelete
    {
        /// <summary>
        /// Webhook unique name id
        /// </summary>
        public string WebHookDefinitionId { get; set; }

        /// <summary>
        /// WebHook data as JSON string.
        /// </summary>
        public string Data { get; set; }

        public bool IsDeleted { get; set; }
    }
}
