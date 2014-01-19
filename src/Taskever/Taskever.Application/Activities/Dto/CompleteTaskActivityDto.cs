using Abp.Users.Dto;
using Taskever.Tasks.Dto;

namespace Taskever.Activities.Dto
{
    public class CompleteTaskActivityDto : ActivityDto
    {
        public UserDto AssignedUser { get; set; }

        public TaskDto Task { get; set; }
    }
}