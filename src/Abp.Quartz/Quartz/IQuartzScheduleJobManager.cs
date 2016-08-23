using System;
using System.Threading.Tasks;
using Abp.Threading.BackgroundWorkers;
using Quartz;

namespace Abp.Quartz.Quartz
{
    /// <summary>
    /// Defines interface of Quartz schedule job manager.
    /// </summary>
    public interface IQuartzScheduleJobManager : IBackgroundWorker
    {
        /// <summary>
        /// Schedules a job to be executed.
        /// </summary>
        /// <typeparam name="TJob">Type of the job</typeparam>
        /// <param name="configureJob">Job specific definitions to build.</param>
        /// <param name="configureTrigger">Job specific trigger options which means calendar or time interval.</param>
        /// <returns></returns>
        Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger) where TJob : IJob;
    }
}