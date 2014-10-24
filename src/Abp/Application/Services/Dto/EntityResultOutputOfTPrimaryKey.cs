namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This <see cref="IOutputDto"/> can be used to send Id of an entity as response from an <see cref="IApplicationService"/> method.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of entity</typeparam>
    public class EntityResultOutput<TPrimaryKey> : EntityDto<TPrimaryKey>, IOutputDto
    {

    }
}