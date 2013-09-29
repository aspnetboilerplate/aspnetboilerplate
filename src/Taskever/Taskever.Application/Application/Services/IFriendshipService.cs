using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Modules.Core.Application.Services.Dto;

namespace Taskever.Application.Services
{
    public interface IFriendshipService : IApplicationService
    {
        IList<UserDto> GetMyFriends();
    }
}