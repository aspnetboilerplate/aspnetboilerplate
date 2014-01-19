using Abp.Application.Services.Dto;

namespace Taskever.Tasks.Dto
{
    public class CreateTaskOutput : IOutputDto
    {
        public TaskDto Task { get; set; }
    }
}