using Abp.Users.Dto;
using Taskever.Tasks.Dto;

namespace Taskever.Activities.Dto
{
    public class CreateTaskActivityDto : ActivityDto
    {
        public UserDto CreatorUser { get; set; }

        public UserDto AssignedUser { get; set; }

        public TaskDto Task { get; set; }
    }
}