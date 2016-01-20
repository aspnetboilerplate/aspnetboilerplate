using System;
using System.Threading.Tasks;

namespace Abp.BackgroundJobs
{
    public interface IBackgroundJobManager
    {
        Task EnqueueAsync<TJob>(object state, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJob;
    }
}