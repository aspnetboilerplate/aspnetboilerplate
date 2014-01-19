using Abp.Application.Services.Dto;

namespace Taskever.Tasks.Dto
{
    public class GetTaskOutput : IOutputDto
    {
        public TaskWithAssignedUserDto Task { get; set; }

        public bool IsEditableByCurrentUser { get; set; }
        
        public bool IsDeletableByCurrentUser { get; set; }
    }
}