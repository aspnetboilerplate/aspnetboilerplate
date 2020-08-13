using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.IdentityServer4vNext
{
    [Table("AbpPersistedGrants")]
    public class PersistedGrantEntity : Entity<string>
    {
        public virtual string Type { get; set; }

        public virtual string SubjectId { get; set; }

        public virtual string SessionId { get; set; }

        public virtual string ClientId { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual DateTime? Expiration { get; set; }

        public virtual DateTime? ConsumedTime { get; set; }

        public virtual string Data { get; set; }
    }
}
