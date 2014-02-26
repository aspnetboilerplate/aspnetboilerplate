using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Security.Users;
using Abp.Utils.Extensions.Collections;
using Castle.Core.Logging;
using Taskever.Friendships;

namespace Taskever.Activities
{
    public class ActivityService : IActivityService
    {
        private readonly IAbpUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUserFollowedActivityRepository _userFollowedActivityRepository;

        public ILogger Logger { get; set; }

        public ActivityService(
            IAbpUserRepository userRepository,
            IFriendshipRepository friendshipRepository,
            IActivityRepository activityRepository,
            IUserFollowedActivityRepository userFollowedActivityRepository)
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
            _activityRepository = activityRepository;
            _userFollowedActivityRepository = userFollowedActivityRepository;
        }

        [UnitOfWork]
        public void AddActivity(Activity activity)
        {
            _activityRepository.Insert(activity);
            CreateUserFollowedActivities(activity);
        }

        [UnitOfWork]
        protected virtual void CreateUserFollowedActivities(Activity activity)
        {
            //TODO: Run this method in a new thread (check connection creation)
            //TODO: Maybe optimized by creating a stored procedure?

            //Get user id's of all actors of this activity
            var actorUserIds = activity.GetActors().Cast<int>().ToList();
            if (actorUserIds.IsNullOrEmpty())
            {
                //No actor of this activity, so, no one will follow it.
                return;
            }

            //Get all followers of these actors
            var followerUserIds = GetFollowersOfUserIds(actorUserIds);

            //Add also actors (if not includes)
            followerUserIds = followerUserIds.Union(actorUserIds).ToList();

            //Get id's of all related users of this activity
            var relatedUserIds = activity.GetRelatedUsers().Cast<int>().ToList();

            //Add also related users (if not includes)
            followerUserIds = followerUserIds.Union(relatedUserIds).ToList();

            //Add one entity for each follower and actor
            foreach (var followerUserId in followerUserIds)
            {
                _userFollowedActivityRepository.Insert(
                    new UserFollowedActivity
                        {
                            User = _userRepository.Load(followerUserId),
                            Activity = activity,
                            IsActor = actorUserIds.Contains(followerUserId),
                            IsRelated = relatedUserIds.Contains(followerUserId)
                        });
            }
        }

        private List<int> GetFollowersOfUserIds(ICollection<int> actorUserIds)
        {
            return _friendshipRepository
                .GetAll()
                .Where(friendship =>
                       actorUserIds.Contains(friendship.Friend.Id)
                       && friendship.FollowActivities)
                .Select(f => f.User.Id).ToList();
        }
    }
}