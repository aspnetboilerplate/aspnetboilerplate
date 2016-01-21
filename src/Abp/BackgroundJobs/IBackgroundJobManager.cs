using System;
using System.Threading.Tasks;
using Abp.Threading.BackgroundWorkers;

namespace Abp.BackgroundJobs
{
    public interface IBackgroundJobManager : IBackgroundWorker
    {
        Task EnqueueAsync<TJob, TArgs>(TArgs state, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJob<TArgs>;
    }
}