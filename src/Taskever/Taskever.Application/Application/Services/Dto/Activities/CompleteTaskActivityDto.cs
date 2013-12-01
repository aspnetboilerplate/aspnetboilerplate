using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services.Dto.Activities
{
    public class CompleteTaskActivityDto : ActivityDto
    {
        public UserDto AssignedUser { get; set; }

        public TaskDto Task { get; set; }
    }
}