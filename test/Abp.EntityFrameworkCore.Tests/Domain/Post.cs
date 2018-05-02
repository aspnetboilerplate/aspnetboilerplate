using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain
{
    public class Post : AuditedEntity<Guid>, ISoftDelete, IMayHaveTenant
    {
        [Required]
        public Blog Blog { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public bool IsDeleted { get; set; }

        public int? TenantId { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public Post()
        {
            Id = Guid.NewGuid();
            Comments = new List<Comment>();
        }

        public Post(Blog blog, string title, string body)
            : this()
        {
            Blog = blog;
            Title = title;
            Body = body;
            Comments = new List<Comment>();
        }
    }
}