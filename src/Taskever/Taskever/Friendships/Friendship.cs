using System;
using Abp.Domain.Entities;
using Abp.Security.Users;
using Taskever.Security.Users;

namespace Taskever.Friendships
{
    public class Friendship : Entity
    {
        public virtual TaskeverUser User { get; set; }

        public virtual TaskeverUser Friend { get; set; }

        public virtual Friendship Pair { get; set; }

        /// <summary>
        /// Is <see cref="User"/> following activities of the <see cref="Friend"/>?
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

        public static Friendship CreateAsRequest(TaskeverUser user, TaskeverUser friend)
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

        public virtual bool IsAccepted()
        {
            return Status == FriendshipStatus.Accepted;
        }

        public virtual bool CanBeAcceptedBy(TaskeverUser acceptorUser)
        {
            switch (Status)
            {
                case FriendshipStatus.Accepted:
                    return true;
                case FriendshipStatus.WaitingApprovalFromUser:
                    return User.Id == acceptorUser.Id;
                case FriendshipStatus.WaitingApprovalFromFriend:
                    return Friend.Id == acceptorUser.Id;
                default:
                    throw new NotImplementedException("Not implemented friendship status: " + Status);
            }
        }

        public virtual void AcceptBy(TaskeverUser acceptorUser)
        {
            if (IsAccepted())
            {
                return;
            }

            if (!CanBeAcceptedBy(acceptorUser))
            {
                throw new ApplicationException("This friendship can not be accepted by this user!");
            }

            Status = FriendshipStatus.Accepted;

            if (Pair == null)
            {
                throw new Exception("Friendship pair is null!");
            }

            Pair.AcceptBy(acceptorUser);
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
