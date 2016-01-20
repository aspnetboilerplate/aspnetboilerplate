using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.BackgroundJobs
{
    public class NullBackgroundJobStore : IBackgroundJobStore
    {
        public Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            return Task.FromResult(0);
        }

        public Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            return Task.FromResult(new List<BackgroundJobInfo>());
        }

        public Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            return Task.FromResult(0);
        }

        public Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            return Task.FromResult(0);
        }
    }
}