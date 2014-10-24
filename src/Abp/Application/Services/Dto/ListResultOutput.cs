namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This class can be used to return a list from an <see cref="IApplicationService"/> method.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="ListResultDto{T}.Items"/> list</typeparam>
    public class ListResultOutput<T> : ListResultDto<T>, IOutputDto
    {

    }
}