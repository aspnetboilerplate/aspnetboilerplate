namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// A shortcut of <see cref="EntityDto{TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    public class EntityDto : EntityDto<int>, IEntityDto
    {

    }
}