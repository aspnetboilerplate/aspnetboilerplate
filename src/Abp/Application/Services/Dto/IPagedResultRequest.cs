using System.ComponentModel.DataAnnotations;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This interface is defined to standardize to request a paged result.
    /// </summary>
    public interface IPagedResultRequest : ILimitedResultRequest
    {
        /// <summary>
        /// Skip count (beginning of the page).
        /// </summary>
        [Range(1, int.MaxValue)]
        int CurrentPage { get; set; }
    }
}