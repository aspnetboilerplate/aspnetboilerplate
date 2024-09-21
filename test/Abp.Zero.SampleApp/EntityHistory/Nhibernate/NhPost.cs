using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory.Nhibernate
{
    public class NhPost : AuditedEntity<Guid>, ISoftDelete, IMayHaveTenant
    {
        [Required]
        public virtual NhBlog Blog { get; set; }

        public virtual ICollection<NhComment> Comments { get; set; }

        [Audited]
        public virtual int BlogId { get; set; }

        public virtual string Title { get; set; }

        public virtual string Body { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual int? TenantId { get; set; }

        public NhPost()
        {
            Id = Guid.NewGuid();
            Comments = new List<NhComment>();
        }

        public NhPost(NhBlog blog, string title, string body)
            : this()
        {
            Blog = blog;
            Title = title;
            Body = body;
            Comments = new List<NhComment>();
        }
    }
}