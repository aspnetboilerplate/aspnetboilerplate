using Castle.Core.Logging;
using Quartz;

namespace Abp.Quartz.Quartz
{
    public class AbpQuartzJobListener : IJobListener
    {
        public string Name { get; } = "AbpJobListener";

        public ILogger Logger { get; set; }

        public AbpQuartzJobListener()
        {
            Logger = NullLogger.Instance;
        }

        public virtual void JobExecutionVetoed(IJobExecutionContext context)
        {
            Logger.Info($"Job {context.JobDetail.JobType.Name} executing operation vetoed...");
        }

        public virtual void JobToBeExecuted(IJobExecutionContext context)
        {
            Logger.Debug($"Job {context.JobDetail.JobType.Name} executing...");
        }

        public virtual void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException == null)
            {
                Logger.Debug($"Job {context.JobDetail.JobType.Name} sucessfully executed.");
            }
            else
            {
                Logger.Error($"Job {context.JobDetail.JobType.Name} failed with exception: {jobException}");
            }
        }
    }
}