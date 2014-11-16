using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This class can be used to return a paged list from an <see cref="IApplicationService"/> method.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="PagedResultDto{T}.Items"/> list</typeparam>
    [Serializable]
    public class PagedResultOutput<T> : PagedResultDto<T>, IOutputDto
    {
        
    }
}