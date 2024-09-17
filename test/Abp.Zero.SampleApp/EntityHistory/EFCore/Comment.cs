using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory.EFCore
{
    [Audited]
    public class Comment : Entity
    {
        public Post Post { get; set; }

        public string Content { get; set; }
    }
}