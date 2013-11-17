using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Activities
{
    public class GetFallowedActivitiesInput : IInputDto, ILimitedResultRequest
    {
        private const int MaxMaxResultCount = 100;

        [Range(1, int.MaxValue)]
        public int FallowerUserId { get; set; }

        [Range(1, MaxMaxResultCount)]
        public int MaxResultCount { get; set; }

        public long BeforeFallowedActivityId { get; set; }

        public GetFallowedActivitiesInput()
        {
            BeforeFallowedActivityId = long.MaxValue;
        }
    }
}