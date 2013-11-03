using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services.Dto
{
    public class TaskWithAssignedUserDto : TaskDto
    {
        public virtual UserDto AssignedUser { get; set; }
    }
}