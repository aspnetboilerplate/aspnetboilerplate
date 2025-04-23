using System;
using System.Reflection;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Threading.BackgroundWorkers;
using Hangfire;

namespace Abp.Hangfire
{
    public class HangfireBackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireBackgroundJobManager(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
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
