using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace Abp.EntityFrameworkCore.Tests.Domain
{
    public class Post : AuditedEntity<Guid>
    {
        [Required]
        public Blog Blog { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}