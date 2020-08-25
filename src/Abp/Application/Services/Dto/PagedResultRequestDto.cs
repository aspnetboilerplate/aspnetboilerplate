using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

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

        public string Keyword { get; set; }
        public string NormalizeKeyword => string.IsNullOrEmpty(Keyword) ? string.Empty : string.Format("%{0}%", Keyword.Normalize());
    }
}