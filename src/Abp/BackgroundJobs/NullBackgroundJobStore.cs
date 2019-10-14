using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.BackgroundJobs
{
    /// <summary>
    /// Null pattern implementation of <see cref="IBackgroundJobStore"/>.
    /// It's used if <see cref="IBackgroundJobStore"/> is not implemented by actual persistent store
    /// and job execution is not enabled (<see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled"/>) for the application.
    /// </summary>
    public class NullBackgroundJobStore : IBackgroundJobStore
    {
        public Task<BackgroundJobInfo> GetAsync(long jobId)
        {
            return Task.FromResult(new BackgroundJobInfo());
        }
        public BackgroundJobInfo Get(long jobId)
        {
            return new BackgroundJobInfo();
        }

        public Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            return Task.FromResult(0);
        }

        public void Insert(BackgroundJobInfo jobInfo)
        {
        }

        public Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            return Task.FromResult(new List<BackgroundJobInfo>());
        }

        public List<BackgroundJobInfo> GetWaitingJobs(int maxResultCount)
        {
            return new List<BackgroundJobInfo>();
        }

        public Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            return Task.FromResult(0);
        }

        public void Delete(BackgroundJobInfo jobInfo)
        {
        }

        public Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            return Task.FromResult(0);
        }

        public void Update(BackgroundJobInfo jobInfo)
        {
        }
    }
}