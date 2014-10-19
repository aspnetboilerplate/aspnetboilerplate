using System.Collections.Generic;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This interface is defined to standardize to return a page of items to clients.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="Items"/> list</typeparam>
    public interface IPagedResult<T> : IHasTotalCount
    {
        /// <summary>
        /// List of items in current page.
        /// </summary>
        IReadOnlyList<T> Items { get; set; }
    }
}