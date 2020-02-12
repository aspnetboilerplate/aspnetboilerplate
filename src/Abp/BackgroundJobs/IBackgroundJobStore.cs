using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.BackgroundJobs
{
    /// <summary>
    /// Defines interface to store/get background jobs.
    /// </summary>
    public interface IBackgroundJobStore
    {
        /// <summary>
        /// Gets a BackgroundJobInfo based on the given jobId.
        /// </summary>
        /// <param name="jobId">The Job Unique Identifier.</param>
        /// <returns>The BackgroundJobInfo object.</returns>
        Task<BackgroundJobInfo> GetAsync(long jobId);

        /// <summary>
        /// Gets a BackgroundJobInfo based on the given jobId.
        /// </summary>
        /// <param name="jobId">The Job Unique Identifier.</param>
        /// <returns>The BackgroundJobInfo object.</returns>
        BackgroundJobInfo Get(long jobId);

        /// <summary>
        /// Inserts a background job.
        /// </summary>
        /// <param name="jobInfo">Job information.</param>
        Task InsertAsync(BackgroundJobInfo jobInfo);

        /// <summary>
        /// Inserts a background job.
        /// </summary>
        /// <param name="jobInfo">Job information.</param>
        void Insert(BackgroundJobInfo jobInfo);

        /// <summary>
        /// Gets waiting jobs. It should get jobs based on these:
        /// Conditions: !IsAbandoned And NextTryTime &lt;= Clock.Now.
        /// Order by: Priority DESC, TryCount ASC, NextTryTime ASC.
        /// Maximum result: <paramref name="maxResultCount"/>.
        /// </summary>
        /// <param name="maxResultCount">Maximum result count.</param>
        Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount);

        /// <summary>
        /// Gets waiting jobs. It should get jobs based on these:
        /// Conditions: !IsAbandoned And NextTryTime &lt;= Clock.Now.
        /// Order by: Priority DESC, TryCount ASC, NextTryTime ASC.
        /// Maximum result: <paramref name="maxResultCount"/>.
        /// </summary>
        /// <param name="maxResultCount">Maximum result count.</param>
        List<BackgroundJobInfo> GetWaitingJobs(int maxResultCount);

        /// <summary>
        /// Deletes a job.
        /// </summary>
        /// <param name="jobInfo">Job information.</param>
        Task DeleteAsync(BackgroundJobInfo jobInfo);

        /// <summary>
        /// Deletes a job.
        /// </summary>
        /// <param name="jobInfo">Job information.</param>
        void Delete(BackgroundJobInfo jobInfo);

        /// <summary>
        /// Updates a job.
        /// </summary>
        /// <param name="jobInfo">Job information.</param>
        Task UpdateAsync(BackgroundJobInfo jobInfo);

        /// <summary>
        /// Updates a job.
        /// </summary>
        /// <param name="jobInfo">Job information.</param>
        void Update(BackgroundJobInfo jobInfo);
    }
}