using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Abp.BackgroundJobs
{
    [Table("AbpBackgroundJobs")]
    public class BackgroundJobInfo : Entity<long>, IHasCreationTime
    {
        public const int MaxJobTypeLength = 512;
        public const int MaxJobArgsLength = 1024 * 1024 * 1024; //1 megabyte

        [Required]
        [StringLength(MaxJobTypeLength)]
        public virtual string JobType { get; set; }

        [Required]
        [MaxLength(MaxJobArgsLength)]
        public virtual string JobArgs { get; set; }

        public virtual short TryCount { get; set; }

        //[Index("IX_IsAbandoned_NextTryTime", 2)]
        public virtual DateTime NextTryTime { get; set; }

        public virtual DateTime? LastTryTime { get; set; }

        public virtual DateTime CreationTime { get; set; }

        //[Index("IX_IsAbandoned_NextTryTime", 1)]
        public virtual bool IsAbandoned { get; set; }
        
        public virtual BackgroundJobPriority Priority { get; set; }

        public BackgroundJobInfo()
        {
            CreationTime = Clock.Now;
            NextTryTime = Clock.Now;
            Priority = BackgroundJobPriority.Normal;
        }

        internal virtual DateTime? CalculateNextTryTime()
        {
            //TODO: This constants can be configurable in the future

            var nextWaitDuration = 60 * (Math.Pow(2, TryCount - 1));
            var nextTryDate = Clock.Now.AddSeconds(nextWaitDuration);

            if (nextTryDate.Subtract(CreationTime).TotalDays > 2.0)
            {
                return null;
            }

            return nextTryDate;
        }
    }
}