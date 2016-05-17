using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Abp.Timing;

namespace Abp.BackgroundJobs
{
    /// <summary>
    /// Represents a background job info that is used to persist jobs.
    /// </summary>
    [Table("AbpBackgroundJobs")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class BackgroundJobInfo : CreationAuditedEntity<long>
    {
        /// <summary>
        /// Maximum length of <see cref="JobType"/>.
        /// Value: 512.
        /// </summary>
        public const int MaxJobTypeLength = 512;

        /// <summary>
        /// Maximum length of <see cref="JobArgs"/>.
        /// Value: 1 MB (1,048,576 bytes).
        /// </summary>
        public const int MaxJobArgsLength = 1024 * 1024;

        /// <summary>
        /// Default duration (as seconds) for the first wait on a failure.
        /// Default value: 60 (1 minutes).
        /// </summary>
        public static int DefaultFirstWaitDuration { get; set; }

        /// <summary>
        /// Default timeout value (as seconds) for a job before it's abandoned (<see cref="IsAbandoned"/>).
        /// Default value: 172,800 (2 days).
        /// </summary>
        public static int DefaultTimeout { get; set; }

        /// <summary>
        /// Default wait factor for execution failures.
        /// This amount is multiplated by last wait time to calculate next wait time.
        /// Default value: 2.0.
        /// </summary>
        public static double DefaultWaitFactor { get; set; }

        /// <summary>
        /// Type of the job.
        /// It's AssemblyQualifiedName of job type.
        /// </summary>
        [Required]
        [StringLength(MaxJobTypeLength)]
        public virtual string JobType { get; set; }

        /// <summary>
        /// Job arguments as JSON string.
        /// </summary>
        [Required]
        [MaxLength(MaxJobArgsLength)]
        public virtual string JobArgs { get; set; }

        /// <summary>
        /// Try count of this job.
        /// A job is re-tried if it fails.
        /// </summary>
        public virtual short TryCount { get; set; }

        /// <summary>
        /// Next try time of this job.
        /// </summary>
        //[Index("IX_IsAbandoned_NextTryTime", 2)]
        public virtual DateTime NextTryTime { get; set; }

        /// <summary>
        /// Last try time of this job.
        /// </summary>
        public virtual DateTime? LastTryTime { get; set; }

        /// <summary>
        /// This is true if this job is continously failed and will not be executed again.
        /// </summary>
        //[Index("IX_IsAbandoned_NextTryTime", 1)]
        public virtual bool IsAbandoned { get; set; }

        /// <summary>
        /// Priority of this job.
        /// </summary>
        public virtual BackgroundJobPriority Priority { get; set; }

        static BackgroundJobInfo()
        {
            DefaultFirstWaitDuration = 60;
            DefaultTimeout = 172800;
            DefaultWaitFactor = 2.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJobInfo"/> class.
        /// </summary>
        public BackgroundJobInfo()
        {
            NextTryTime = Clock.Now;
            Priority = BackgroundJobPriority.Normal;
        }

        /// <summary>
        /// Calculates next try time if a job fails.
        /// Returns null if it will not wait anymore and job should be abandoned.
        /// </summary>
        /// <returns></returns>
        protected internal virtual DateTime? CalculateNextTryTime()
        {
            var nextWaitDuration = DefaultFirstWaitDuration * (Math.Pow(DefaultWaitFactor, TryCount - 1));
            var nextTryDate = LastTryTime.HasValue
                ? LastTryTime.Value.AddSeconds(nextWaitDuration)
                : Clock.Now.AddSeconds(nextWaitDuration);

            if (nextTryDate.Subtract(CreationTime).TotalSeconds > DefaultTimeout)
            {
                return null;
            }

            return nextTryDate;
        }
    }
}