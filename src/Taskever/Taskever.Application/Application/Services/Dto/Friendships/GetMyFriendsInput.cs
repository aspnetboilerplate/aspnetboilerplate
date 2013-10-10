using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class GetMyFriendsInput : IInputDto
    {
        public bool CanAssignTask { get; set; }
    }
}