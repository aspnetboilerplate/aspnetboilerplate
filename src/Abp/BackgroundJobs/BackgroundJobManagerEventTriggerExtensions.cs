using System;
using System.Threading.Tasks;
using Abp.Events.Bus;

namespace Abp.BackgroundJobs
{
    public static class BackgroundJobManagerEventTriggerExtensions
    {
        public static Task EnqueueEventAsync<TEvent>(this IBackgroundJobManager backgroundJobManager,
            TEvent e,BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TEvent:EventData
        {
            return backgroundJobManager.EnqueueAsync<EventTriggerAsyncBackgroundJob<TEvent>,TEvent>(e,priority,delay);
        }
    }
}