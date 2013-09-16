using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories;
using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Services.Dto;
using Abp.Modules.Core.Services.Impl;
using Taskever.Entities;

namespace Taskever.Services.Impl
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IRepository<User> _userRepository;

        private readonly IRepository<Friendship> _friendshipRepository;

        public FriendshipService(IRepository<User> userRepository, IRepository<Friendship> friendshipRepository)
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public IList<UserDto> GetMyFriends()
        {
            const int currentUserId = 1;
            var list = _friendshipRepository.Query(friendships =>
                                            {
                                                var qr = from user in _userRepository.GetAll()
                                                         join friendship in friendships on user.Id equals friendship.Friend.Id
                                                         where friendship.User.Id == currentUserId && friendship.Status == FriendshipStatus.Accepted
                                                         select user;
                                                return qr.ToList();
                                            });

            return list.MapIList<User, UserDto>();
        }
    }
}