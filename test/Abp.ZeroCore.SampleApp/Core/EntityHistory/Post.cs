using System;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    public class Post : AuditedEntity<Guid>, ISoftDelete, IMayHaveTenant
    {
        [Required]
        public Blog Blog { get; set; }

        [Audited]
        public int BlogId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public bool IsDeleted { get; set; }

        public int? TenantId { get; set; }

        public Post()
        {
            Id = Guid.NewGuid();
        }

        public Post(Blog blog, string title, string body)
            : this()
        {
            Blog = blog;
            Title = title;
            Body = body;
        }
    }
}