using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyProject.News
{
    public class News : FullAuditedEntity<long>, IPassivable, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public virtual string Title { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [Required]
        public virtual string Intro { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        [Required]
        public virtual string Content { get; set; }

        //public virtual DateTime CreationTime { get; set; }

        //public virtual long? CreatorUserId { get; set; }

        //public virtual DateTime? LastModificationTime { get; set; }

        //public virtual long? LastModifierUserId { get; set; }

        //public virtual bool IsDeleted { get; set; }

        //public virtual long? DeleterUserId { get; set; }

        //public virtual DateTime? DeletionTime { get; set; }

        public virtual bool IsActive { get; set; }

        public News()
        {
            CreationTime = DateTime.Now;
            IsActive = true;
        }

    }
}
