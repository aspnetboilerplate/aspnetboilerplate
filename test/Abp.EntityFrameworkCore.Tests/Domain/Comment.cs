using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain
{
    public class Comment : Entity
    {
        public Post Post { get; set; }

        public string Content { get; set; }
    }
}