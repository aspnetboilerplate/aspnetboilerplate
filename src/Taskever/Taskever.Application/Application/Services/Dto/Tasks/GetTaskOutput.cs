using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class GetTaskOutput : IOutputDto
    {
        public TaskWithAssignedUserDto Task { get; set; }

        public bool IsEditableByCurrentUser { get; set; }
    }
}