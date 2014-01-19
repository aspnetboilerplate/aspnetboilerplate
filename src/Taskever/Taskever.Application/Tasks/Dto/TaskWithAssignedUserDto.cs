using Abp.Users.Dto;

namespace Taskever.Tasks.Dto
{
    public class TaskWithAssignedUserDto : TaskDto
    {
        public virtual UserDto AssignedUser { get; set; }
    }
}