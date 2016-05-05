using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// A shortcut of <see cref="IEntityDto{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    public interface IEntityDto : IEntityDto<Guid>
    {
    }
}