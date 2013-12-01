using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services.Dto.Activities
{
    public class CreateTaskActivityDto : ActivityDto
    {
        public UserDto CreatorUser { get; set; }

        public UserDto AssignedUser { get; set; }

        public TaskDto Task { get; set; }
    }
}