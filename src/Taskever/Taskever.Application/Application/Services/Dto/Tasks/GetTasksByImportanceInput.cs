using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class GetTasksByImportanceInput : IInputDto, ILimitedResultRequest
    {
        private const int MaxMaxResultCount = 100;

        [Range(1, int.MaxValue)]
        public int AssignedUserId { get; set; }

        [Range(1, MaxMaxResultCount)]
        public int MaxResultCount { get; set; }

        public GetTasksByImportanceInput()
        {
            MaxResultCount = MaxMaxResultCount;
        }
    }
}