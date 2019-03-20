using Abp.Auditing;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory
{
    [Audited]
    public class Comment : Entity
    {
        public virtual Post Post { get; set; }

        public string Content { get; set; }
    }
}
