namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// A shortcut of <see cref="EntityDto{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public class EntityDto : EntityDto<int>
    {

    }
}