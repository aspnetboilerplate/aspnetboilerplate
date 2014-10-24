namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This <see cref="IOutputDto"/> can be used to send Id of an entity as response from an <see cref="IApplicationService"/> method.
    /// </summary>
    public class EntityResultOutput : EntityResultOutput<int>, IEntityDto
    {

    }
}