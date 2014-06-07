using Abp.Mapping;
using Abp.Security.Users;
using Abp.UI;
using Taskever.Activities.Dto;
using Taskever.Friendships;
using Taskever.Security.Users;

namespace Taskever.Activities
{
    public class UserActivityAppService : IUserActivityAppService
    {
        private readonly ITaskeverUserRepository _userRepository;
        private readonly IUserFollowedActivityRepository _followedActivityRepository;
        private readonly IFriendshipDomainService _friendshipDomainService;

        public UserActivityAppService(ITaskeverUserRepository userRepository, IUserFollowedActivityRepository followedActivityRepository, IFriendshipDomainService friendshipDomainService)
        {
            _userRepository = userRepository;
            _followedActivityRepository = followedActivityRepository;
            _friendshipDomainService = friendshipDomainService;
        }

        public GetFollowedActivitiesOutput GetFollowedActivities(GetFollowedActivitiesInput input)
        {
            var currentUser = _userRepository.Load(AbpUser.CurrentUserId.Value);
            var user = _userRepository.Load(input.UserId);

            //Can see activities of this user?
            if (currentUser.Id != user.Id && !_friendshipDomainService.HasFriendship(user, currentUser))
            {
                throw new UserFriendlyException("Can not see activities of this user!");
            }

            //TODO: Think on private activities? When a private task is created or completed?

            var activities = _followedActivityRepository.Getactivities(input.UserId,input.IsActor, input.BeforeId, input.MaxResultCount);

            return new GetFollowedActivitiesOutput
                       {
                           Activities = activities.MapIList<UserFollowedActivity, UserFollowedActivityDto>()
                       };
        }
    }
}