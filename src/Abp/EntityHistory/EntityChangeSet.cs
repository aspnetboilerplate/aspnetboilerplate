using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.EntityHistory
{
    [Table("AbpEntityChangeSets")]
    public class EntityChangeSet : Entity<long>, IHasCreationTime, IMayHaveTenant, IExtendableObject
    {
        /// <summary>
        /// Maximum length of <see cref="BrowserInfo"/> property.
        /// </summary>
        public const int MaxBrowserInfoLength = 512;

        /// <summary>
        /// Maximum length of <see cref="ClientIpAddress"/> property.
        /// </summary>
        public const int MaxClientIpAddressLength = 64;

        /// <summary>
        /// Maximum length of <see cref="ClientName"/> property.
        /// </summary>
        public const int MaxClientNameLength = 128;

        /// <summary>
        /// Maximum length of <see cref="Reason"/> property.
        /// </summary>
        public const int MaxReasonLength = 256;

        /// <summary>
        /// Browser information if this entity is changed in a web request.
        /// </summary>
        [StringLength(MaxBrowserInfoLength)]
        public virtual string BrowserInfo { get; set; }

        /// <summary>
        /// IP address of the client.
        /// </summary>
        [StringLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        /// Name (generally computer name) of the client.
        /// </summary>
        [StringLength(MaxClientNameLength)]
        public virtual string ClientName { get; set; }

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// A JSON formatted string to extend the containing object.
        /// </summary>
        public virtual string ExtensionData { get; set; }

        /// <summary>
        /// ImpersonatorTenantId.
        /// </summary>
        public virtual int? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// ImpersonatorUserId.
        /// </summary>
        public virtual long? ImpersonatorUserId { get; set; }

        /// <summary>
        /// Reason for this change set.
        /// </summary>
        [StringLength(MaxReasonLength)]
        public virtual string Reason { get; set; }

        /// <summary>
        /// TenantId.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// UserId.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        /// Entity changes grouped in this change set.
        /// </summary>
        public virtual IList<EntityChange> EntityChanges { get; set; }

        public EntityChangeSet()
        {
            EntityChanges = new List<EntityChange>();
        }
    }
}
