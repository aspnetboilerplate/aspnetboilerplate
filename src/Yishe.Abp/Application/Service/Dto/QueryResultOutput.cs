using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Application.Services.Dto
{
    /// <summary>
    /// This class can be used to return a paged list from an <see cref="IApplicationService"/> method.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="PagedResultDto{T}.Items"/> list</typeparam>
    [Serializable]
    public class QueryResultOutput<T> : PagedResultDto<T>, IOutputDto
    {
        /// <summary>
        /// Creates a new <see cref="QueryResultOutput{T}"/> object.
        /// </summary>
        public QueryResultOutput()
        {

        }

        /// <summary>
        /// Creates a new <see cref="QueryResultOutput{T}"/> object.
        /// </summary>
        /// <param name="totalCount">Total count of Items</param>
        /// <param name="items">List of items in current page</param>
        public QueryResultOutput(int totalCount, IReadOnlyList<T> items)
            : base(totalCount, items)
        {

        }
    }
}
