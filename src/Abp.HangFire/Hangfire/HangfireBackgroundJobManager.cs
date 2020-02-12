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

        public Task<string> EnqueueAsync<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJob<TArgs>
        {
            string jobUniqueIdentifier = string.Empty;

            if (!delay.HasValue)
            {
                jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            }
            else
            {
                jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
            }

            return Task.FromResult(jobUniqueIdentifier);
        }

        public string Enqueue<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJob<TArgs>
        {
            string jobUniqueIdentifier = string.Empty;

            if (!delay.HasValue)
            {
                jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            }
            else
            {
                jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
            }

            return jobUniqueIdentifier;
        }

        public Task<bool> DeleteAsync(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            bool successfulDeletion = HangfireBackgroundJob.Delete(jobId);
            return Task.FromResult(successfulDeletion);
        }

        public bool Delete(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            bool successfulDeletion = HangfireBackgroundJob.Delete(jobId);
            return successfulDeletion;
        }
    }
}