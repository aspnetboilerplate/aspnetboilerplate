using System.Linq;
using Abp.Domain.Uow;
using System;
using Abp.Modules.Core.Data.Repositories;
using Castle.Core.Logging;
using Taskever.Data.Repositories;
using Taskever.Domain.Business.Acitivities;
using Taskever.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Taskever.Application.Services.Impl
{
    public class ActivityService : IActivityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUserActivityRepository _userActivityRepository;

        public ILogger Logger { get; set; }

        public ActivityService(
            IUserRepository userRepository, 
            IFriendshipRepository friendshipRepository, 
            IActivityRepository activityRepository, 
            IUserActivityRepository userActivityRepository)
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
            _activityRepository = activityRepository;
            _userActivityRepository = userActivityRepository;
        }

        [UnitOfWork]
        public void AddActivity(ActivityData activityData)
        {
            var activity = new Activity
                              {
                                  Data = activityData.SerializeData(),
                                  ActorUser = _userRepository.Load(activityData.GetActorUserId),
                                  Action = activityData.Action
                              };
            _activityRepository.Insert(activity);

            //TODO: Make this in a new thread (also check connection creation for the new thread)
            CreateUserActivities(activity);
        }

        protected virtual void CreateUserActivities(Activity activity)
        {
            var fallowerUserIds = _friendshipRepository.Query(q => q.Where(f => f.Friend.Id == activity.ActorUser.Id && f.FallowActivities).Select(f => f.User.Id));
            foreach (var fallowerUserId in fallowerUserIds)
            {
                _userActivityRepository.Insert(
                    new UserActivity
                        {
                            User = _userRepository.Load(fallowerUserId),
                            Activity = activity
                        });
            }
        }
    }
}