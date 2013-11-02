using System;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto.Friendships;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Policies;

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

        public virtual GetFriendshipsOutput GetFriendships(GetFriendshipsInput input)
        {
            //TODO: Check if current user can see friendships of the the requested user!
            var friendships = _friendshipRepository.GetAllWithFriendUser(input.UserId, input.Status, input.CanAssignTask);
            return new GetFriendshipsOutput { Friendships = friendships.MapIList<Friendship, FriendshipDto>() };
        }

        [UnitOfWork]
        public virtual ChangeFriendshipPropertiesOutput ChangeFriendshipProperties(ChangeFriendshipPropertiesInput input)
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

        public virtual SendFriendshipRequestOutput SendFriendshipRequest(SendFriendshipRequestInput input)
        {
            var friendUser = _userRepository.Query(q => q.FirstOrDefault(user => user.EmailAddress == input.EmailAddress));
            if (friendUser == null)
            {
                throw new AbpUserFriendlyException("Can not find a user with email address: " + input.EmailAddress);
            }

            var currentUser = _userRepository.Load(User.CurrentUserId);

            //TODO: Check if they are already friends!

            var friendShip = Friendship.CreateAsRequest(currentUser, friendUser);
            _friendshipRepository.Insert(friendShip);
            
            return new SendFriendshipRequestOutput();
        }

        [UnitOfWork] //TODO: Need UnitOfWork, I think no!
        public virtual RemoveFriendshipOutput RemoveFriendship(RemoveFriendshipInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var friendship = _friendshipRepository.Get(input.Id); //TODO: Call GetOrNull and throw a specific exception?

            if(!_friendshipPolicy.CanRemoveFriendship(currentUser, friendship)) //TODO: Maybe this method can throw exception!
            {
                throw new ApplicationException("Can not remove this friendship!"); //TODO: User friendliy exception
            }

            _friendshipRepository.Delete(friendship);

            return new RemoveFriendshipOutput();
        }

        [UnitOfWork]
        public virtual AcceptFriendshipOutput AcceptFriendship(AcceptFriendshipInput input)
        {
            var friendship = _friendshipRepository.Get(input.Id); //TODO: Call GetOrNull and throw a specific exception?
            var currentUser = _userRepository.Load(User.CurrentUserId);

            friendship.AcceptBy(currentUser);

            return new AcceptFriendshipOutput();
        }

        public virtual RejectFriendshipOutput RejectFriendship(RejectFriendshipInput input)
        {
            RemoveFriendship(new RemoveFriendshipInput {Id = input.Id});
            return new RejectFriendshipOutput();
        }
    }
}