using System;
using Abp.Domain.Entities.Auditing;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// 对于大多数使用的主键类型（<see cref ="int"/>）的<see cref ="AuditedEntityDto {TPrimaryKey}"/>的快捷方式。
    /// </summary>
    [Serializable]
    public abstract class AuditedEntityDto : AuditedEntityDto<int>
    {

    }

    /// <summary>
    /// This class can be inherited for simple Dto objects those are used for entities implement <see cref="IAudited{TUser}"/> interface.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of primary key</typeparam>
    [Serializable]
    public abstract class AuditedEntityDto<TPrimaryKey> : CreationAuditedEntityDto<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        public long? LastModifierUserId { get; set; }
    }
}