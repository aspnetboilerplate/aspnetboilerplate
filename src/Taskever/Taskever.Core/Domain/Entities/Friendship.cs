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

        public virtual Friendship Pair { get; set; }

        /// <summary>
        /// Is <see cref="User"/> fallowing activities of the <see cref="Friend"/>?
        /// </summary>
        public virtual bool FollowActivities { get; set; }

        /// <summary>
        /// Can <see cref="Friend"/> assign tasks to the <see cref="User"/>?
        /// </summary>
        public virtual bool CanAssignTask { get; set; }

        public virtual FriendshipStatus Status { get; set; }

        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// The last time the <see cref="User"/> visited to the <see cref="Friend"/>.
        /// </summary>
        public virtual DateTime LastVisitTime { get; set; }

        public Friendship()
        {
            CreationTime = DateTime.Now;
            LastVisitTime = DateTime.Now;
            CanAssignTask = true;
            FollowActivities = true;
        }

        public virtual void AcceptBy(User acceptorUser)
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

            if (Pair == null)
            {
                throw new Exception("Friendship pair is null!");
            }

            Pair.AcceptBy(acceptorUser);
        }

        public static Friendship CreateAsRequest(User user, User friend)
        {
            if (user.Id == friend.Id)
            {
                throw new Exception("A user can not send request to the same user!");
            }

            var friendShip = new Friendship
            {
                User = user,
                Status = FriendshipStatus.WaitingApprovalFromFriend,
                Friend = friend
            };

            friendShip.CreatePair();
            return friendShip;
        }

        private void CreatePair()
        {
            Pair = new Friendship
            {
                User = Friend,
                Status = FriendshipStatus.WaitingApprovalFromUser,
                Friend = User,
                Pair = this
            };
        }
    }
}
