using System;
using System.ComponentModel.DataAnnotations;

using Abp.Domain.Entities.Auditing;

namespace Abp.EntityFrameworkCore.Dapper.Tests.Domain;

public class Post : AuditedEntity<Guid>
{
    public Post()
    {
    }

    public Post(Blog blog, string title, string body)
    {
        Blog = blog;
        Title = title;
        Body = body;
    }

    public Post(int blogId, string title, string body)
    {
        BlogId = blogId;
        Title = title;
        Body = body;
    }

    [Required]
    public Blog Blog { get; set; }

    public Comment Comment { get; set; }

    public int CommentId { get; set; }

    public int BlogId { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }
}