using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class GetFriendshipsOutput : IOutputDto
    {
        public IList<FriendshipDto> Friendships { get; set; }
    }
}