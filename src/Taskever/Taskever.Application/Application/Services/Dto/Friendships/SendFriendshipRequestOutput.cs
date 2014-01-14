using Abp.Application.Services.Dto;
using Taskever.Friendships;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class SendFriendshipRequestOutput : IOutputDto
    {
        public FriendshipStatus Status { get; set; }
    }
}