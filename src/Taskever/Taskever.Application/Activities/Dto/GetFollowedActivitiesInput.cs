using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Activities.Dto
{
    public class GetFollowedActivitiesInput : IInputDto, ILimitedResultRequest
    {
        private const int MaxMaxResultCount = 100;

        [Range(1, int.MaxValue)]
        public long UserId { get; set; }

        public bool? IsActor { get; set; }

        [Range(1, long.MaxValue)]
        public long BeforeId { get; set; }

        [Range(1, MaxMaxResultCount)]
        public int MaxResultCount { get; set; }

        public GetFollowedActivitiesInput()
        {
            BeforeId = long.MaxValue;
            MaxResultCount = MaxMaxResultCount;
        }
    }
}