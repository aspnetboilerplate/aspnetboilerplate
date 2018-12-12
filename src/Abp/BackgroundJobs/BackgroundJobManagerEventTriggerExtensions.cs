using System;
using System.Threading.Tasks;
using Abp.Events.Bus;

namespace Abp.BackgroundJobs
{
    public static class BackgroundJobManagerEventTriggerExtensions
    {
        
        public static Task EnqueueEventAsync<T>(this IBackgroundJobManager t,
            T e,BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where T:EventData
        {
            return t.EnqueueAsync<EventTriggerAsyncBackgroundJob<T>,T>(e,priority,delay);
        }
    }
}