using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Hangfire.Configuration;
using Abp.Threading.BackgroundWorkers;
using Castle.Core.Logging;
using Hangfire;

namespace Abp.Hangfire
{
    public class HangfireBackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager, ISingletonDependency
    {
        public ILogger Logger { get; set; } 

        private readonly IAbpHangfireConfiguration _configuration;

        public HangfireBackgroundJobManager(IAbpHangfireConfiguration configuration)
        {
            _configuration = configuration;

            Logger = NullLogger.Instance;
        }

        public override void Start()
        {
            base.Start();

            if (_configuration.Server == null)
            {
                _configuration.Server = new BackgroundJobServer();
            }
        }
        
        public override void WaitToStop()
        {
            try
            {
                _configuration.Server.Dispose();
            }
            catch(Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
         
            base.WaitToStop();
        }

        public Task EnqueueAsync<TJob>(object state, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJob
        {
            BackgroundJob.Enqueue<TJob>(job => job.Execute(state));

            return Task.FromResult(0);
        }
    }
}
