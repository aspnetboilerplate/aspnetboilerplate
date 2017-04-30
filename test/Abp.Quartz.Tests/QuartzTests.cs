using System;
using System.Threading;

using Abp.Dependency;
using Abp.Quartz.Quartz;
using Abp.Quartz.Quartz.Configuration;
using Abp.TestBase;

using Quartz;

using Shouldly;

using Xunit;

namespace Abp.Quartz.Tests
{
    public class QuartzTests : AbpIntegratedTestBase<AbpQuartzTestModule>
    {
        private readonly IAbpQuartzConfiguration _abpQuartzConfiguration;
        private readonly IQuartzScheduleJobManager _quartzScheduleJobManager;

        public QuartzTests()
        {
            _quartzScheduleJobManager = LocalIocManager.Resolve<IQuartzScheduleJobManager>();
            _abpQuartzConfiguration = LocalIocManager.Resolve<IAbpQuartzConfiguration>();

            ScheduleJobs();
        }

        private void ScheduleJobs()
        {
            _quartzScheduleJobManager.ScheduleAsync<HelloJob>(
                job =>
                {
                    job.WithDescription("HelloJobDescription")
                       .WithIdentity("HelloJobKey");
                },
                trigger =>
                {
                    trigger.WithIdentity("HelloJobTrigger")
                           .WithDescription("HelloJobTriggerDescription")
                           .WithSimpleSchedule(schedule => schedule.WithRepeatCount(5).WithInterval(TimeSpan.FromSeconds(1)).Build())
                           .StartNow();
                });

            _quartzScheduleJobManager.ScheduleAsync<GoodByeJob>(
                job =>
                {
                    job.WithDescription("GoodByeJobDescription")
                       .WithIdentity("GoodByeJobKey");
                },
                trigger =>
                {
                    trigger.WithIdentity("GoodByeJobTrigger")
                           .WithDescription("GoodByeJobTriggerDescription")
                           .WithSimpleSchedule(schedule => schedule.WithRepeatCount(5).WithInterval(TimeSpan.FromSeconds(1)).Build())
                           .StartNow();
                });
        }

        [Fact]
        public void QuartzScheduler_Jobs_ShouldBeRegistered()
        {
            _abpQuartzConfiguration.Scheduler.ShouldNotBeNull();
            _abpQuartzConfiguration.Scheduler.IsStarted.ShouldBe(true);
            _abpQuartzConfiguration.Scheduler.CheckExists(JobKey.Create("HelloJobKey")).ShouldBe(true);
            _abpQuartzConfiguration.Scheduler.CheckExists(JobKey.Create("GoodByeJobKey")).ShouldBe(true);
        }

        [Fact]
        public void QuartzScheduler_Jobs_ShouldBeExecuted_With_SingletonDependency()
        {
            var helloDependency = LocalIocManager.Resolve<IHelloDependency>();
            var goodByeDependency = LocalIocManager.Resolve<IGoodByeDependency>();

            //Wait for execution!
            Thread.Sleep(TimeSpan.FromSeconds(5));

            helloDependency.ExecutionCount.ShouldBeGreaterThan(0);
            goodByeDependency.ExecutionCount.ShouldBeGreaterThan(0);
        }
    }

    [DisallowConcurrentExecution]
    public class HelloJob : JobBase, ITransientDependency
    {
        private readonly IHelloDependency _helloDependency;

        public HelloJob(IHelloDependency helloDependency)
        {
            _helloDependency = helloDependency;
        }

        public override void Execute(IJobExecutionContext context)
        {
            _helloDependency.ExecutionCount++;
        }
    }

    [DisallowConcurrentExecution]
    public class GoodByeJob : JobBase, ITransientDependency
    {
        private readonly IGoodByeDependency _goodByeDependency;

        public GoodByeJob(IGoodByeDependency goodByeDependency)
        {
            _goodByeDependency = goodByeDependency;
        }

        public override void Execute(IJobExecutionContext context)
        {
            _goodByeDependency.ExecutionCount++;
        }
    }

    public interface IHelloDependency
    {
        int ExecutionCount { get; set; }
    }

    public interface IGoodByeDependency
    {
        int ExecutionCount { get; set; }
    }

    public class HelloDependency : IHelloDependency, ISingletonDependency
    {
        public int ExecutionCount { get; set; }
    }

    public class GoodByeDependency : IGoodByeDependency, ISingletonDependency
    {
        public int ExecutionCount { get; set; }
    }
}
