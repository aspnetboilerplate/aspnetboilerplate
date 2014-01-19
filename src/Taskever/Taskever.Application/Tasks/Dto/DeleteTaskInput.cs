using Abp.Application.Services.Dto;

namespace Taskever.Tasks.Dto
{
    public class DeleteTaskInput :IInputDto
    {
        public int Id { get; set; }
    }

    public class DeleteTaskOutput : IOutputDto
    {
        public int Id { get; set; }
    }
}