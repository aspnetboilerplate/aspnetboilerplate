using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class ChangeFriendshipPropertiesInput : IInputDto
    {
        public int Id { get; set; }

        public bool? FollowActivities { get; set; }

        public bool? CanAssignTask { get; set; }
    }
}