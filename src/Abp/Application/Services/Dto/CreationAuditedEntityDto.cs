using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    ///  A shortcut of <see cref="CreationAuditedEntityDto"/> for most used primary key type (<see cref="Guid"/>).
    /// </summary>
    [Serializable]
    public abstract class CreationAuditedEntityDto : CreationAuditedEntityDto<Guid>
    {
    }
}