using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// A shortcut of <see cref="AuditedEntityDto{TPrimaryKey}"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class AuditedEntityDto : AuditedEntityDto<Guid>
    {
    }
}