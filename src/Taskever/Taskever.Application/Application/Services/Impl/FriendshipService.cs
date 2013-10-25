using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto.Friendships;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Application.Services.Impl
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IUserRepository _userRepository;

        private readonly IFriendshipRepository _friendshipRepository;

        public FriendshipService(IUserRepository userRepository, IFriendshipRepository friendshipRepository)
        { 
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public GetFriendshipsOutput GetFriendships(GetFriendshipsInput input)
        {
            //Check if current user can see friendships of the the requested user!
            var friendships = _friendshipRepository.GetAllWithFriendUser(input.UserId, input.CanAssignTask);
            return new GetFriendshipsOutput { Friendships = friendships.MapIList<Friendship, FriendshipDto>() };
        }
    }
}