using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.FriendshipService
{
    public class GetMyFriendsInput : IInputDto
    {
        public bool CanAssignTask { get; set; }
    }
}