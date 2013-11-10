using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class GetFriendshipsByMostActiveOutput : IOutputDto
    {
        public IList<FriendshipDto> Friendships { get; set; }
    }
}