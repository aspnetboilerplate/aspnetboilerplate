using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto.Friendships;
using Taskever.Domain.Entities;

namespace Taskever.Application.Services.Impl
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IUserRepository _userRepository;

        private readonly IRepository<Friendship> _friendshipRepository;

        public FriendshipService(IUserRepository userRepository, IRepository<Friendship> friendshipRepository)
        { 
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public IList<UserDto> GetMyFriends(GetMyFriendsInput input)
        {
            const int currentUserId = 1;
            var list = _friendshipRepository.Query(friendships =>
                                            {
                                                var qr = from user in _userRepository.GetAll()
                                                         join friendship in friendships on user.Id equals friendship.Friend.Id
                                                         where friendship.User.Id == currentUserId && friendship.Status == FriendshipStatus.Accepted
                                                         select new { user, friendship };

                                                if (input.CanAssignTask)
                                                {
                                                    qr = qr.Where(_ => _.friendship.CanAssignTask);
                                                }

                                                return qr.Select(_ => _.user).ToList();
                                            });

            return list.MapIList<User, UserDto>();
        }
    }
}