using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Quartz.Configuration;
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
        }

        private async Task ScheduleJobs()
        {
            await _quartzScheduleJobManager.ScheduleAsync<HelloJob>(
                job =>
                {
                    job.WithDescription("HelloJobDescription")
                       .WithIdentity("HelloJobKey");
                },
                trigger =>
                {
                    trigger.WithIdentity("HelloJobTrigger")
                           .WithDescription("HelloJobTriggerDescription")
                           .WithSimpleSchedule(schedule => schedule.WithRepeatCount(5).WithInterval(TimeSpan.FromSeconds(1)))
                           .StartNow();
                });

            await _quartzScheduleJobManager.ScheduleAsync<GoodByeJob>(
                job =>
                {
                    job.WithDescription("GoodByeJobDescription")
                       .WithIdentity("GoodByeJobKey");
                },
                trigger =>
                {
                    trigger.WithIdentity("GoodByeJobTrigger")
                           .WithDescription("GoodByeJobTriggerDescription")
                           .WithSimpleSchedule(schedule => schedule.WithRepeatCount(5).WithInterval(TimeSpan.FromSeconds(1)))
                           .StartNow();
                });
        }

        [Fact]
        public async Task QuartzScheduler_Jobs_ShouldBe_Registered_And_Executed_With_SingletonDependency()
        {
            // There should be only one test case in this project, or the unit test may fail in AppVeyor
            await ScheduleJobs();

            var helloDependency = LocalIocManager.Resolve<IHelloDependency>();
            var goodByeDependency = LocalIocManager.Resolve<IGoodByeDependency>();

            _abpQuartzConfiguration.Scheduler.ShouldNotBeNull();
            _abpQuartzConfiguration.Scheduler.IsStarted.ShouldBe(true);
            (await _abpQuartzConfiguration.Scheduler.CheckExists(JobKey.Create("HelloJobKey"))).ShouldBe(true);
            (await _abpQuartzConfiguration.Scheduler.CheckExists(JobKey.Create("GoodByeJobKey"))).ShouldBe(true);

            //Wait for execution!
            await Task.Delay(TimeSpan.FromSeconds(5));

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

        public override Task Execute(IJobExecutionContext context)
        {
            _helloDependency.ExecutionCount++;

            return Task.CompletedTask;
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
        
        public override Task Execute(IJobExecutionContext context)
        {
            _goodByeDependency.ExecutionCount++;

            return Task.CompletedTask;
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
