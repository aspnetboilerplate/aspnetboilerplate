using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Modules.Core.Application.Services.Dto;
using Taskever.Application.Services.Dto.Friendships;

namespace Taskever.Application.Services
{
    public interface IFriendshipService : IApplicationService
    {
        IList<UserDto> GetMyFriends(GetMyFriendsInput input);
    }
}