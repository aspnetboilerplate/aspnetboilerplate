using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Modules.Core.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto.Friendships;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Policies;
using Taskever.Domain.Services;

namespace Taskever.Application.Services.Impl
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IUserRepository _userRepository;

        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IFriendshipPolicy _friendshipPolicy;

        public FriendshipService(IUserRepository userRepository, IFriendshipRepository friendshipRepository, IFriendshipPolicy friendshipPolicy)
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
            _friendshipPolicy = friendshipPolicy;
        }

        public GetFriendshipsOutput GetFriendships(GetFriendshipsInput input)
        {
            //TODO: Check if current user can see friendships of the the requested user!
            var friendships = _friendshipRepository.GetAllWithFriendUser(input.UserId, input.CanAssignTask);
            return new GetFriendshipsOutput { Friendships = friendships.MapIList<Friendship, FriendshipDto>() };
        }

        [UnitOfWork]
        public ChangeFriendshipPropertiesOutput ChangeFriendshipProperties(ChangeFriendshipPropertiesInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var friendShip = _friendshipRepository.Get(input.Id); //TODO: Call GetOrNull and throw a specific exception?
            
            if (!_friendshipPolicy.CanChangeFriendshipProperties(currentUser, friendShip))
            {
                throw new ApplicationException("Can not change properties of this friendship!");
            }

            //TODO: Implement mappings using Auto mapper!

            if (input.CanAssignTask.HasValue)
            {
                friendShip.CanAssignTask = input.CanAssignTask.Value;
            }

            if (input.FallowActivities.HasValue)
            {
                friendShip.FallowActivities = input.FallowActivities.Value;
            }

            _friendshipRepository.Update(friendShip);

            return new ChangeFriendshipPropertiesOutput();
        }
    }
}