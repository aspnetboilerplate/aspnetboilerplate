namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This <see cref="IInputDto"/> can be used to send Id of an entity to an <see cref="IApplicationService"/> method.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of entity</typeparam>
    public class EntityRequestInput<TPrimaryKey> : EntityDto<TPrimaryKey>, IInputDto
    {

    }
}