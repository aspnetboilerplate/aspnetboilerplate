using System;
using System.ComponentModel.DataAnnotations;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// Simply implements <see cref="IPagedResultRequest"/>.
    /// </summary>
    [Serializable]
    public class PagedResultRequestDto : LimitedResultRequestDto, IPagedResultRequest
    {
        [Range(1, int.MaxValue)]
        public virtual int CurrentPage { get; set; } = 1;
    }
}