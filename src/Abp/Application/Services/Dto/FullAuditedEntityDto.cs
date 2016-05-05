using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// A shortcut of <see cref="FullAuditedEntityDto{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class FullAuditedEntityDto : FullAuditedEntityDto<Guid>
    {
    }
}