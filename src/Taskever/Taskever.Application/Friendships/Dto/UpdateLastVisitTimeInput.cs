using Abp.Application.Services.Dto;

namespace Taskever.Friendships.Dto
{
    public class UpdateLastVisitTimeInput : IInputDto
    {
        public long FriendUserId { get; set; }
    }
}