using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Modules.Core.Data.Repositories;
using Abp.Utils.Extensions;
using Castle.Core.Logging;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Domain.Services.Impl
{
    public class ActivityService : IActivityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUserFollowedActivityRepository _userFollowedActivityRepository;

        public ILogger Logger { get; set; }

        public ActivityService(
            IUserRepository userRepository,
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
            CreateUserFallowedActivities(activity);
        }

        [UnitOfWork]
        protected virtual void CreateUserFallowedActivities(Activity activity)
        {
            //TODO: Run this method in a new thread (check connection creation)

            //Get user id's of all actors of this activity
            var actorUserIds = activity.GetActors().Select(user => user.Id).ToList();
            if (actorUserIds.IsNullOrEmpty())
            {
                //No actor of this activity, so, no one will fallow it.
                return;
            }

            //Get all fallowers of these actors
            var followerUserIds = GetFollowersOfUserIds(actorUserIds);

            //Add also actors (if not includes)
            followerUserIds = followerUserIds.Union(actorUserIds).ToList();

            //Add one entity for each fallower and actor
            foreach (var fallowerUserId in followerUserIds)
            {
                _userFollowedActivityRepository.Insert(
                    new UserFollowedActivity
                        {
                            User = _userRepository.Load(fallowerUserId),
                            Activity = activity,
                            IsActor = actorUserIds.Contains(fallowerUserId)
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