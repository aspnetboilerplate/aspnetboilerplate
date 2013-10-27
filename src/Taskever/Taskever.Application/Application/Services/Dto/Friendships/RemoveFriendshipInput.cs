using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class RemoveFriendshipInput :IInputDto
    {
        public int Id { get; set; }
    }
}