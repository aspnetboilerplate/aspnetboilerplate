using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Entities
{
    public class Friendship : Entity
    {
        public virtual User User { get; set; }
        
        public virtual User Friend { get; set; }

        /// <summary>
        /// Can <see cref="Friend"/> assign tasks to the <see cref="User"/>
        /// </summary>
        public virtual bool CanAssignTask { get; set; }

        public virtual FriendshipStatus Status { get; set; }
    }
}
