using Abp.Application.Services.Dto;

namespace Taskever.Friendships.Dto
{
    public class RemoveFriendshipInput :IInputDto
    {
        public int Id { get; set; } //TODO: Get UserId and FriendId instead of Id?
    }
}