using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Timing;

namespace Abp.BackgroundJobs
{
    public class InMemoryBackgroundJobStore : IBackgroundJobStore
    {
        private readonly List<BackgroundJobInfo> _jobs;

        public InMemoryBackgroundJobStore()
        {
            _jobs = new List<BackgroundJobInfo>();
        }

        public Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            _jobs.Add(jobInfo);
            
            return Task.FromResult(0);
        }

        public Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            var waitingJobs = _jobs
                .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.TryCount)
                .ThenBy(t => t.NextTryTime)
                .Take(1000)
                .ToList();

            return Task.FromResult(waitingJobs);
        }

        public Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            _jobs.Remove(jobInfo);

            return Task.FromResult(0);
        }

        public Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            return Task.FromResult(0);            
        }
    }
}