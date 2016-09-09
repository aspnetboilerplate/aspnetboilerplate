using System.ComponentModel.DataAnnotations;

namespace Abp.Application.Services.Dto
{
    public class LimitedResultRequest : ILimitedResultRequest
    {
        [Range(1, int.MaxValue)]
        public virtual int MaxResultCount { get; set; } = 10;
    }
}