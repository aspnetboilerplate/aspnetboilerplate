using System.Linq;
using Abp.Domain.Uow;
using Abp.Modules.Core.Data.Repositories;
using Castle.Core.Logging;
using Taskever.Data.Repositories;
using Taskever.Domain.Business.Acitivities;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Services.Impl
{
    public class ActivityService : IActivityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUserFallowedActivityRepository _userFallowedActivityRepository;

        public ILogger Logger { get; set; }

        public ActivityService(
            IUserRepository userRepository, 
            IFriendshipRepository friendshipRepository, 
            IActivityRepository activityRepository, 
            IUserFallowedActivityRepository userFallowedActivityRepository)
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
            _activityRepository = activityRepository;
            _userFallowedActivityRepository = userFallowedActivityRepository;
        }

        [UnitOfWork]
        public void AddActivity(ActivityInfo activityData)
        {
            var activity = new Activity
                              {
                                  Data = activityData.SerializeData(),
                                  ActorUser = _userRepository.Load(activityData.GetActorUserId()),
                                  Action = activityData.Action
                              };
            _activityRepository.Insert(activity);

            //TODO: Make this in a new thread (also check connection creation for the new thread)
            CreateUserFallowedActivities(activity);
        }

        protected virtual void CreateUserFallowedActivities(Activity activity)
        {
            var fallowerUserIds = _friendshipRepository.Query(q => q.Where(f => f.Friend.Id == activity.ActorUser.Id && f.FallowActivities).Select(f => f.User.Id));
            foreach (var fallowerUserId in fallowerUserIds)
            {
                _userFallowedActivityRepository.Insert(
                    new UserFallowedActivity
                        {
                            User = _userRepository.Load(fallowerUserId),
                            Activity = activity
                        });
            }
        }
    }
}