using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Activities
{
    public class GetUserActivitiesInput : IInputDto, ILimitedResultRequest
    {
        private const int MaxMaxResultCount = 100;

        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
        
        [Range(1, MaxMaxResultCount)]
        public int MaxResultCount { get; set; }

        public int BeforeActivityId { get; set; }

        public GetUserActivitiesInput()
        {
            BeforeActivityId = int.MaxValue;
        }
    }
}