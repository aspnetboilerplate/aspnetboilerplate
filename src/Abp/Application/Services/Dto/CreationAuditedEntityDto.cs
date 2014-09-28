namespace Abp.Application.Services.Dto
{
    /// <summary>
    ///  A shortcut of <see cref="CreationAuditedEntityDto"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    public abstract class CreationAuditedEntityDto : CreationAuditedEntityDto<int>
    {
        
    }
}