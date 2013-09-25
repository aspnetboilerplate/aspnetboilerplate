using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Modules.Core.Services.Dto;

namespace Taskever.Services
{
    public interface IFriendshipService : IApplicationService
    {
        IList<UserDto> GetMyFriends();
    }
}