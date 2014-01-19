using Abp.Application.Services.Dto;

namespace Taskever.Friendships.Dto
{
    public class SendFriendshipRequestOutput : IOutputDto
    {
        public FriendshipStatus Status { get; set; }
    }
}