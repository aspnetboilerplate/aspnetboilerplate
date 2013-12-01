using Abp.Modules.Core.Application.Services.Dto.Users;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class TaskWithAssignedUserDto : TaskDto
    {
        public virtual UserDto AssignedUser { get; set; }
    }
}