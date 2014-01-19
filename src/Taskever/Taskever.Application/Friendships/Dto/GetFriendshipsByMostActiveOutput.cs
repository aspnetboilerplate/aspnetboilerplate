using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Friendships.Dto
{
    public class GetFriendshipsByMostActiveOutput : IOutputDto
    {
        public IList<FriendshipDto> Friendships { get; set; }
    }
}