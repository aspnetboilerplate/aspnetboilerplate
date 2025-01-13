using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Hangfire.Configuration;
using Abp.Threading.BackgroundWorkers;
using Hangfire;

namespace Abp.Hangfire
{
    public class HangfireBackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager
    {
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IAbpHangfireConfiguration _hangfireConfiguration;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireBackgroundJobManager(
            IBackgroundJobConfiguration backgroundJobConfiguration,
            IAbpHangfireConfiguration hangfireConfiguration,
            IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _hangfireConfiguration = hangfireConfiguration;
            _backgroundJobClient = backgroundJobClient;
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

        public virtual Task<string> EnqueueAsync<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJobBase<TArgs>
        {
            string jobUniqueIdentifier = string.Empty;

            if (!delay.HasValue)
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = _backgroundJobClient.Enqueue<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args));
                }
                else
                {
                    jobUniqueIdentifier = _backgroundJobClient.Enqueue<TJob>(job => ((IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args));
                }
            }
            else
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = _backgroundJobClient.Schedule<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args), delay.Value);
                }
                else
                {
                    jobUniqueIdentifier = _backgroundJobClient.Schedule<TJob>(job => ((IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args), delay.Value);
                }
            }

            return Task.FromResult(jobUniqueIdentifier);
        }

        public virtual string Enqueue<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJobBase<TArgs>
        {
            string jobUniqueIdentifier = string.Empty;

            if (!delay.HasValue)
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = _backgroundJobClient.Enqueue<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args));
                }
                else
                {
                    jobUniqueIdentifier = _backgroundJobClient.Enqueue<TJob>(job => ((IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args));
                }
            }
            else
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = _backgroundJobClient.Schedule<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args), delay.Value);
                }
                else
                {
                    jobUniqueIdentifier = _backgroundJobClient.Schedule<TJob>(job => ((IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args), delay.Value);
                }
            }

            return jobUniqueIdentifier;
        }

        public virtual Task<bool> DeleteAsync(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            bool successfulDeletion = _backgroundJobClient.Delete(jobId);
            return Task.FromResult(successfulDeletion);
        }

        public virtual bool Delete(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            bool successfulDeletion = _backgroundJobClient.Delete(jobId);
            return successfulDeletion;
        }
    }
}
