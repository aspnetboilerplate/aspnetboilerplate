using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Hangfire.Configuration;
using Abp.Threading.BackgroundWorkers;
using Hangfire;
using HangfireBackgroundJob = Hangfire.BackgroundJob;

namespace Abp.Hangfire
{
    public class HangfireBackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager
    {
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IAbpHangfireConfiguration _hangfireConfiguration;

        public HangfireBackgroundJobManager(
            IBackgroundJobConfiguration backgroundJobConfiguration,
            IAbpHangfireConfiguration hangfireConfiguration)
        {
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _hangfireConfiguration = hangfireConfiguration;
        }

        public override void Start()
        {
            base.Start();

            if (_hangfireConfiguration.Server == null && _backgroundJobConfiguration.IsJobExecutionEnabled)
            {
                _hangfireConfiguration.Server = new BackgroundJobServer();
            }
        }

        public override void WaitToStop()
        {
            if (_hangfireConfiguration.Server != null)
            {
                try
                {
                    _hangfireConfiguration.Server.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            base.WaitToStop();
        }

        public Task EnqueueAsync<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJob<TArgs>
        {
            if (!delay.HasValue)
                HangfireBackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            else
                HangfireBackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
            return Task.FromResult(0);
        }
    }
}
