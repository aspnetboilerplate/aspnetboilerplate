using System;
using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Entities
{
    public class Friendship : Entity
    {
        public virtual User User { get; set; }

        public virtual User Friend { get; set; }

        /// <summary>
        /// Is <see cref="User"/> fallowing activities of the <see cref="Friend"/>?
        /// </summary>
        public virtual bool FallowActivities { get; set; }

        /// <summary>
        /// Can <see cref="Friend"/> assign tasks to the <see cref="User"/>?
        /// </summary>
        public virtual bool CanAssignTask { get; set; }

        public virtual FriendshipStatus Status { get; set; }

        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        public Friendship()
        {
            CreationTime = DateTime.Now;
            CanAssignTask = true;
            FallowActivities = true;
        }

        public virtual void Accept(User acceptorUser)
        {
            switch (Status)
            {
                case FriendshipStatus.Accepted:
                    return;
                case FriendshipStatus.WaitingApprovalFromUser:
                    if (User.Id != acceptorUser.Id)
                    {
                        throw new ApplicationException("Can not accept this friendship!"); //TODO: Better exceptions
                    }
                    break;
                case FriendshipStatus.WaitingApprovalFromFriend:
                    if (Friend.Id != acceptorUser.Id)
                    {
                        throw new ApplicationException("Can not accept this friendship!"); //TODO: Better exceptions
                    }
                    break;
                default:
                    throw new NotImplementedException("Not implemented friendship status: " + Status);
            }

            Status = FriendshipStatus.Accepted;
        }
    }
}
