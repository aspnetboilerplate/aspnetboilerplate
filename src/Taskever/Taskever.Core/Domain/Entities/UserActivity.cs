using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Entities
{
    public class UserActivity : Entity<long>
    {
        public virtual User User { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
