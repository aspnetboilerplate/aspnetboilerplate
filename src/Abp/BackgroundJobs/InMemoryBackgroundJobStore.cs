using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Timing;

namespace Abp.BackgroundJobs
{
    /// <summary>
    /// In memory implementation of <see cref="IBackgroundJobStore"/>.
    /// It's used if <see cref="IBackgroundJobStore"/> is not implemented by actual persistent store
    /// and job execution is enabled (<see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled"/>) for the application.
    /// </summary>
    public class InMemoryBackgroundJobStore : IBackgroundJobStore
    {
        private readonly ConcurrentDictionary<long, BackgroundJobInfo> _jobs;
        private long _lastId;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryBackgroundJobStore"/> class.
        /// </summary>
        public InMemoryBackgroundJobStore()
        {
            _jobs = new ConcurrentDictionary<long, BackgroundJobInfo>();
        }

        public Task<BackgroundJobInfo> GetAsync(long jobId)
        {
            return Task.FromResult(_jobs[jobId]);
        }

        public BackgroundJobInfo Get(long jobId)
        {
            return _jobs[jobId];
        }

        public Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            jobInfo.Id = Interlocked.Increment(ref _lastId);
            _jobs[jobInfo.Id] = jobInfo;

            return Task.FromResult(0);
        }

        public void Insert(BackgroundJobInfo jobInfo)
        {
            jobInfo.Id = Interlocked.Increment(ref _lastId);
            _jobs[jobInfo.Id] = jobInfo;
        }

        public Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            var waitingJobs = _jobs.Values
                .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.TryCount)
                .ThenBy(t => t.NextTryTime)
                .Take(maxResultCount)
                .ToList();

            return Task.FromResult(waitingJobs);
        }

        public List<BackgroundJobInfo> GetWaitingJobs(int maxResultCount)
        {
            var waitingJobs = _jobs.Values
                .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.TryCount)
                .ThenBy(t => t.NextTryTime)
                .Take(maxResultCount)
                .ToList();

            return waitingJobs;
        }

        public Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            _jobs.TryRemove(jobInfo.Id, out _);

            return Task.FromResult(0);
        }

        public void Delete(BackgroundJobInfo jobInfo)
        {
            _jobs.TryRemove(jobInfo.Id, out _);
        }

        public Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            if (jobInfo.IsAbandoned)
            {
                return DeleteAsync(jobInfo);
            }

            return Task.FromResult(0);
        }

        public void Update(BackgroundJobInfo jobInfo)
        {
            if (jobInfo.IsAbandoned)
            {
                Delete(jobInfo);
            }
        }
    }
}