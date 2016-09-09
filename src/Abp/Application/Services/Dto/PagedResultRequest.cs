using System.ComponentModel.DataAnnotations;

namespace Abp.Application.Services.Dto
{
    public class PagedResultRequest : LimitedResultRequest, IPagedResultRequest
    {
        [Range(0, int.MaxValue)]
        public virtual int SkipCount { get; set; }
    }
}