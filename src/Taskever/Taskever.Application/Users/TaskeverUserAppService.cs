using Abp.Exceptions;
using Abp.Mapping;
using Abp.Users;
using Abp.Users.Dto;
using Taskever.Friendships;
using Taskever.Users.Dto;

namespace Taskever.Users
{
    public class TaskeverUserAppService : ITaskeverUserAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskeverUserPolicy _taskeverUserPolicy;
        private readonly IFriendshipRepository _friendshipRepository;

        public TaskeverUserAppService(IUserRepository userRepository, ITaskeverUserPolicy taskeverUserPolicy, IFriendshipRepository friendshipRepository)
        {
            _userRepository = userRepository;
            _taskeverUserPolicy = taskeverUserPolicy;
            _friendshipRepository = friendshipRepository;
        }

        public GetUserProfileOutput GetUserProfile(GetUserProfileInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);

            var profileUser = _userRepository.Get(input.UserId);
            if (profileUser == null)
            {
                throw new AbpUserFriendlyException("Can not found the user!");
            }

            if (profileUser.Id != currentUser.Id)
            {
                var friendship = _friendshipRepository.GetOrNull(currentUser.Id, input.UserId);
                if (friendship == null)
                {
                    return new GetUserProfileOutput { CanNotSeeTheProfile = true, User = profileUser.MapTo<UserDto>() }; //TODO: Is it true to send user informations?
                }

                if (friendship.Status != FriendshipStatus.Accepted)
                {
                    return new GetUserProfileOutput { CanNotSeeTheProfile = true, SentFriendshipRequest = true, User = profileUser.MapTo<UserDto>() };
                }
            }

            return new GetUserProfileOutput { User = profileUser.MapTo<UserDto>() };
        }
    }
}