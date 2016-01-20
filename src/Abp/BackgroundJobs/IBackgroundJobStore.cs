using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.BackgroundJobs
{
    public interface IBackgroundJobStore
    {
        Task InsertAsync(BackgroundJobInfo jobInfo);

        Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount);
        
        Task DeleteAsync(BackgroundJobInfo jobInfo);
        
        Task UpdateAsync(BackgroundJobInfo jobInfo);
    }
}