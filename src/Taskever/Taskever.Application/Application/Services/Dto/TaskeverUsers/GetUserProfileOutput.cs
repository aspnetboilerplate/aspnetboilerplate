using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services.Dto.TaskeverUsers
{
    public class GetUserProfileOutput :IOutputDto
    {
        public bool CanNotSeeTheProfile { get; set; }

        public UserDto User { get; set; }

        public IList<TaskDto> CurrentTasks { get; set; }

        public IList<TaskDto> CompletedTasks { get; set; }
    }
}