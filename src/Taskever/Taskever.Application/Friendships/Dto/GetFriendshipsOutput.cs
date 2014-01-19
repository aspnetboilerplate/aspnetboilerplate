using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Friendships.Dto
{
    public class GetFriendshipsOutput : IOutputDto
    {
        public IList<FriendshipDto> Friendships { get; set; }
    }
}