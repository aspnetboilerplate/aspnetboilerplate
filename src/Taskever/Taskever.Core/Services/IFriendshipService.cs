using System.Collections.Generic;
using Abp.Modules.Core.Services.Dto;
using Abp.Services;

namespace Taskever.Services
{
    public interface IFriendshipService : IService
    {
        IList<UserDto> GetMyFriends();
    }
}